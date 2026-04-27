using System.Threading.Channels;
using CBBSimulator.Core.Models;
using CBBSimulator.Core.Simulation;
using CBBSimulator.Data.Services;
using CBBSimulator.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.BackgroundServices;

public record SimulationRequest(
    string SimulationId,
    string Type, // "game", "tournament", "season"
    Dictionary<string, string> Parameters);

public class SimulationQueue
{
    private readonly Channel<SimulationRequest> _channel;

    public SimulationQueue(int capacity = 100)
    {
        _channel = Channel.CreateBounded<SimulationRequest>(
            new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public ValueTask EnqueueAsync(SimulationRequest request, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(request, ct);

    public bool TryEnqueue(SimulationRequest request)
        => _channel.Writer.TryWrite(request);

    public IAsyncEnumerable<SimulationRequest> DequeueAllAsync(CancellationToken ct = default)
        => _channel.Reader.ReadAllAsync(ct);
}

public class SimulationWorkerService : BackgroundService
{
    private readonly SimulationQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SimulationWorkerService> _logger;

    public SimulationWorkerService(
        SimulationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<SimulationWorkerService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var maxParallelism = Environment.ProcessorCount * 2;
        _logger.LogInformation(
            "Simulation worker started. Max parallelism: {Max}", maxParallelism);

        await Parallel.ForEachAsync(
            _queue.DequeueAllAsync(stoppingToken),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = maxParallelism,
                CancellationToken = stoppingToken
            },
            async (request, ct) =>
            {
                _logger.LogInformation(
                    "Processing {Type} simulation {Id}",
                    request.Type, request.SimulationId);

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    await RunSimulationAsync(scope.ServiceProvider, request, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Simulation {Id} failed", request.SimulationId);
                }
            });
    }

    private async Task RunSimulationAsync(
        IServiceProvider services,
        SimulationRequest request,
        CancellationToken ct)
    {
        switch (request.Type)
        {
            case "game":
                await RunGameAsync(services, request, ct);
                break;
            default:
                _logger.LogWarning(
                    "Unknown simulation type: {Type} (id {Id})",
                    request.Type, request.SimulationId);
                break;
        }
    }

    private static async Task RunGameAsync(
        IServiceProvider services,
        SimulationRequest request,
        CancellationToken ct)
    {
        var teamData = services.GetRequiredService<ITeamDataService>();
        var engine = services.GetRequiredService<IGameEngine>();
        var hub = services.GetRequiredService<IHubContext<GameHub>>();

        var awayName = request.Parameters["away"];
        var homeName = request.Parameters["home"];
        var speed = Enum.TryParse<SimSpeed>(
            request.Parameters.GetValueOrDefault("speed"), out var s)
                ? s : SimSpeed.Medium;

        var away = await teamData.GetTeamByNameAsync(awayName);
        var home = await teamData.GetTeamByNameAsync(homeName);

        var group = hub.Clients.Group(request.SimulationId);

        if (away is null || home is null)
        {
            await group.SendAsync(
                "Error",
                new { message = $"Team not found: '{(away is null ? awayName : homeName)}'" },
                ct);
            return;
        }

        await foreach (var evt in engine.SimulateGameAsync(away, home, speed, ct))
        {
            var (method, payload) = MapEvent(evt);
            await group.SendAsync(method, payload, ct);
        }
    }

    private static (string Method, object Payload) MapEvent(GameEvent evt) => evt switch
    {
        PossessionEvent      p  => ("PossessionResult",  p),
        ClockAdvancedEvent   c  => ("ClockAdvanced",     c),
        ScoreUpdatedEvent    s  => ("ScoreUpdate",       s),
        PeriodStartedEvent   ps => ("PeriodStarted",     ps),
        PeriodEndedEvent     pe => ("PeriodEnded",       pe),
        HalftimeEvent        h  => ("Halftime",          h),
        OvertimeStartedEvent o  => ("OvertimeStarted",   o),
        GameCompletedEvent   g  => ("GameOver",          g),
        _                       => ("GameEvent",         evt)
    };
}
