using CBBSimulator.Core.Models;
using CBBSimulator.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class GameHub : Hub
{
    private readonly ITeamDataService _teamData;

    public GameHub(ITeamDataService teamData)
    {
        _teamData = teamData;
    }

    public async Task StartGame(string awayTeam, string homeTeam, SimSpeed speed)
    {
        var gameId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        // TODO: Queue simulation to SimulationWorkerService
        // Worker will stream events to this group via IHubContext
        await Clients.Caller.SendAsync("GameCreated", gameId);
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task PauseGame(string gameId)
    {
        // TODO: Signal the simulation worker to pause
    }

    public async Task ResumeGame(string gameId)
    {
        // TODO: Signal the simulation worker to resume
    }

    public async Task SetSpeed(string gameId, SimSpeed speed)
    {
        // TODO: Signal the simulation worker to change speed
    }
}
