using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;
using CBBSimulator.Core.Simulation;
using CBBSimulator.Data.Services;

namespace CBBSimulator.Web.Endpoints;

public static class SimulationEndpoints
{
    public static void MapSimulationEndpoints(this WebApplication app)
    {
        app.MapPost("/api/simulate/quick", async (
            QuickSimRequest request,
            ITeamDataService teamData,
            SimulationConfig config) =>
        {
            var away = await teamData.GetTeamByNameAsync(request.AwayTeam);
            var home = await teamData.GetTeamByNameAsync(request.HomeTeam);

            if (away is null || home is null)
                return Results.NotFound("One or both teams not found");

            var engine = new GameEngine(config);
            MatchupResult? result = null;

            await foreach (var evt in engine.SimulateGameAsync(away, home, SimSpeed.Instant))
            {
                if (evt is GameCompletedEvent completed)
                    result = completed.Result;
            }

            return Results.Ok(result);
        });

        app.MapGet("/api/health", (ITeamDataService teamData) =>
        {
            return Results.Ok(new
            {
                Status = "Healthy",
                teamData.LastRefreshed
            });
        });
    }
}

public record QuickSimRequest(string AwayTeam, string HomeTeam);
