using CBBSimulator.Web.BackgroundServices;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class TournamentHub : Hub
{
    private readonly SimulationQueue _queue;

    public TournamentHub(SimulationQueue queue)
    {
        _queue = queue;
    }

    public async Task StartTournament(int speedMs)
    {
        var tournamentId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId);
        await Clients.Caller.SendAsync("TournamentCreated", tournamentId);

        var request = new SimulationRequest(
            tournamentId,
            "tournament",
            new Dictionary<string, string>
            {
                ["mode"] = "auto",
                ["speed"] = speedMs.ToString()
            });

        await _queue.EnqueueAsync(request, Context.ConnectionAborted);
    }

    public async Task StartCustomTournament(string bracketJson, int speedMs)
    {
        var tournamentId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId);
        await Clients.Caller.SendAsync("TournamentCreated", tournamentId);

        var request = new SimulationRequest(
            tournamentId,
            "tournament",
            new Dictionary<string, string>
            {
                ["mode"] = "custom",
                ["bracket"] = bracketJson,
                ["speed"] = speedMs.ToString()
            });

        await _queue.EnqueueAsync(request, Context.ConnectionAborted);
    }

    public async Task JoinTournament(string tournamentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId);
    }
}
