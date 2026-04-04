using CBBSimulator.Core.Models;
using CBBSimulator.Data.Services;
using Microsoft.AspNetCore.SignalR;

namespace CBBSimulator.Web.Hubs;

public class SeasonHub : Hub
{
    private readonly ITeamDataService _teamData;

    public SeasonHub(ITeamDataService teamData)
    {
        _teamData = teamData;
    }

    public async Task StartSeason(SimSpeed speed)
    {
        var seasonId = Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, seasonId);

        // TODO: Queue season simulation to worker service
        await Clients.Caller.SendAsync("SeasonCreated", seasonId);
    }

    public async Task JoinSeason(string seasonId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, seasonId);
    }

    public async Task SetSpeed(string seasonId, SimSpeed speed)
    {
        // TODO: Signal the simulation worker to change speed
    }
}
