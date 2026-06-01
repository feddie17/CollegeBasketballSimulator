using CBBSimulator.Web.BackgroundServices;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class SeasonHub : Hub
{
    private readonly SimulationQueue _queue;

    public SeasonHub(SimulationQueue queue)
    {
        _queue = queue;
    }

    public async Task StartSeason(int speedMs)
    {
        var seasonId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, seasonId);
        await Clients.Caller.SendAsync("SeasonCreated", seasonId);

        var request = new SimulationRequest(
            seasonId,
            "season",
            new Dictionary<string, string>
            {
                ["speed"] = speedMs.ToString()
            });

        await _queue.EnqueueAsync(request, Context.ConnectionAborted);
    }

    public async Task JoinSeason(string seasonId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, seasonId);
    }
}
