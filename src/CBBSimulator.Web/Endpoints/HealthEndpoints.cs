using CBBSimulator.Data.Services;

namespace CBBSimulator.Web.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", async (ITeamDataService teamData) =>
        {
            var teams = await teamData.GetAllTeamsAsync();
            var lastRefreshed = teamData.LastRefreshed;
            var cacheAgeMinutes = lastRefreshed == default
                ? (double?)null
                : Math.Round((DateTime.UtcNow - lastRefreshed).TotalMinutes, 1);

            return Results.Ok(new
            {
                status = "healthy",
                teamCount = teams.Count,
                teamsWithFtp = teams.Count(t => t.FTP > 0),
                lastRefreshed = lastRefreshed == default ? (DateTime?)null : lastRefreshed,
                cacheAgeMinutes
            });
        });
    }
}
