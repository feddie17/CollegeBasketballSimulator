using CBBSimulator.Core.Models;
using CBBSimulator.Web.BackgroundServices;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class GameHub : Hub
{
    private readonly SimulationQueue _queue;

    public GameHub(SimulationQueue queue)
    {
        _queue = queue;
    }

    public async Task StartGame(string awayTeam, string homeTeam, SimSpeed speed)
    {
        var gameId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Caller.SendAsync("GameCreated", gameId);

        var request = new SimulationRequest(
            SimulationId: gameId,
            Type: "game",
            Parameters: new Dictionary<string, string>
            {
                ["away"] = awayTeam,
                ["home"] = homeTeam,
                ["speed"] = speed.ToString()
            });

        await _queue.EnqueueAsync(request, Context.ConnectionAborted);
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    public Task PauseGame(string gameId) => Task.CompletedTask;
    public Task ResumeGame(string gameId) => Task.CompletedTask;
    public Task SetSpeed(string gameId, SimSpeed speed) => Task.CompletedTask;
}
