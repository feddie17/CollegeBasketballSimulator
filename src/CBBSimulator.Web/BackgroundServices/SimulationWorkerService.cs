using System.Threading.Channels;

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
                    // TODO: Resolve engine from DI scope, run simulation,
                    // push events via IHubContext to the simulation's SignalR group
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
        // TODO: Implement based on request.Type
        await Task.CompletedTask;
    }
}
