using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Simulation;
using CBBSimulator.Data.Services;
using CBBSimulator.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

/// <summary>
/// Drives an interactive season: the client starts a season, then advances it a
/// day or a week at a time, or runs it to the end at a chosen speed and stops it
/// whenever they like. Session state lives in <see cref="SeasonSessionManager"/>.
/// </summary>
public class SeasonHub : Hub
{
    private readonly ITeamDataService _teamData;
    private readonly SimulationConfig _config;
    private readonly SeasonSessionManager _sessions;
    private readonly IHubContext<SeasonHub> _hubContext;
    private readonly ILogger<SeasonHub> _logger;

    public SeasonHub(
        ITeamDataService teamData,
        SimulationConfig config,
        SeasonSessionManager sessions,
        IHubContext<SeasonHub> hubContext,
        ILogger<SeasonHub> logger)
    {
        _teamData = teamData;
        _config = config;
        _sessions = sessions;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task StartSeason()
    {
        var teams = (await _teamData.GetAllTeamsAsync()).ToList();
        var schedule = (await _teamData.GetScheduleAsync()).ToList();

        if (teams.Count == 0 || schedule.Count == 0)
        {
            await Clients.Caller.SendAsync("Error", new { message = "No team or schedule data loaded" });
            return;
        }

        var seasonId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, seasonId);

        var simulation = new SeasonSimulation(teams, schedule, _config);
        _sessions.Add(new SeasonSession
        {
            SeasonId = seasonId,
            ConnectionId = Context.ConnectionId,
            Simulation = simulation
        });

        await Clients.Caller.SendAsync("SeasonCreated", seasonId);
        await Clients.Caller.SendAsync("SeasonStarted", simulation.BuildStartedEvent());
    }

    public Task SimulateDay(string seasonId)
        => StepAsync(seasonId, (sim, ct) => sim.SimulateNextDayAsync(ct));

    public Task SimulateWeek(string seasonId)
        => StepAsync(seasonId, (sim, ct) => sim.SimulateNextWeekAsync(ct));

    public Task SimulateToEnd(string seasonId, int speedMs)
    {
        if (!_sessions.TryGet(seasonId, out var session))
        {
            return Clients.Caller.SendAsync("Error", new { message = "Season not found" });
        }

        session.CancelRun();
        session.RunCts = new CancellationTokenSource();
        var token = session.RunCts.Token;

        // Run in the background so a subsequent StopSeason invocation isn't queued
        // behind this long-running loop. Events are pushed to the group directly.
        _ = Task.Run(async () =>
        {
            try
            {
                while (!session.Simulation.IsComplete && !token.IsCancellationRequested)
                {
                    await session.Gate.WaitAsync(token);
                    List<SeasonEvent> events;
                    try
                    {
                        events = await session.Simulation.SimulateNextDayAsync(token);
                    }
                    finally
                    {
                        session.Gate.Release();
                    }

                    await SendEventsAsync(seasonId, events, token);

                    if (speedMs > 0)
                    {
                        await Task.Delay(speedMs, token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Stopped by the user; expected.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Season run {Id} failed", seasonId);
            }
        }, token);

        return Task.CompletedTask;
    }

    public Task StopSeason(string seasonId)
    {
        if (_sessions.TryGet(seasonId, out var session))
        {
            session.CancelRun();
        }
        return Task.CompletedTask;
    }

    public Task JoinSeason(string seasonId)
        => Groups.AddToGroupAsync(Context.ConnectionId, seasonId);

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _sessions.RemoveByConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    private async Task StepAsync(
        string seasonId,
        Func<SeasonSimulation, CancellationToken, Task<List<SeasonEvent>>> step)
    {
        if (!_sessions.TryGet(seasonId, out var session))
        {
            await Clients.Caller.SendAsync("Error", new { message = "Season not found" });
            return;
        }

        await session.Gate.WaitAsync(Context.ConnectionAborted);
        try
        {
            var events = await step(session.Simulation, Context.ConnectionAborted);
            await SendEventsAsync(seasonId, events, Context.ConnectionAborted);
        }
        finally
        {
            session.Gate.Release();
        }
    }

    private async Task SendEventsAsync(string seasonId, List<SeasonEvent> events, CancellationToken ct)
    {
        foreach (var evt in events)
        {
            var (method, payload) = MapSeasonEvent(evt);
            await _hubContext.Clients.Group(seasonId).SendAsync(method, payload, ct);
        }
    }

    private static (string Method, object Payload) MapSeasonEvent(SeasonEvent evt) => evt switch
    {
        SeasonStartedEvent s       => ("SeasonStarted", s),
        SeasonDayCompletedEvent d  => ("SeasonDayCompleted", d),
        SeasonWeekCompletedEvent w => ("SeasonWeekCompleted", w),
        SeasonCompletedEvent c     => ("SeasonCompleted", c),
        _                          => ("SeasonEvent", evt)
    };
}
