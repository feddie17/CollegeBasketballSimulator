using CBBSimulator.Core.Models;
using CBBSimulator.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class TournamentHub : Hub
{
    private readonly ITeamDataService _teamData;

    public TournamentHub(ITeamDataService teamData)
    {
        _teamData = teamData;
    }

    public async Task StartTournament(SimSpeed speed)
    {
        var tournamentId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId);

        // TODO: Queue tournament simulation to worker service
        await Clients.Caller.SendAsync("TournamentCreated", tournamentId);
    }

    public async Task JoinTournament(string tournamentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId);
    }

    public async Task SetSpeed(string tournamentId, SimSpeed speed)
    {
        // TODO: Signal the simulation worker to change speed
    }
}
