using CBBSimulator.Data.Services;

namespace CBBSimulator.Web.Endpoints;

public static class TeamsEndpoints
{
    public static void MapTeamsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/teams");

        group.MapGet("/", async (ITeamDataService teamData) =>
        {
            var teams = await teamData.GetAllTeamsAsync();
            return Results.Ok(teams.Select(t => new
            {
                t.Rank, t.Name, t.Conference,
                t.Wins, t.Losses, t.ConfWins, t.ConfLosses,
                t.Record, t.Sos
            }));
        });

        group.MapGet("/{name}", async (string name, ITeamDataService teamData) =>
        {
            var team = await teamData.GetTeamByNameAsync(name);
            return team is null ? Results.NotFound() : Results.Ok(team);
        });

        group.MapGet("/search", async (string q, ITeamDataService teamData) =>
        {
            var teams = await teamData.SearchTeamsAsync(q);
            return Results.Ok(teams.Select(t => new
            {
                t.Rank, t.Name, t.Conference,
                t.Wins, t.Losses, t.ConfWins, t.ConfLosses,
                t.Record, t.Sos
            }));
        });
    }
}
