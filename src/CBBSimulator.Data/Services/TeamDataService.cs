using CBBSimulator.Core.Models;
using CBBSimulator.Data.Caching;

namespace CBBSimulator.Data.Services;

public class TeamDataService : ITeamDataService
{
    private readonly TeamDataCache _cache;

    public TeamDataService(TeamDataCache cache)
    {
        _cache = cache;
    }

    public DateTime LastRefreshed => _cache.LastRefreshed;

    public async Task<IReadOnlyList<CollegeModel>> GetAllTeamsAsync()
    {
        return await _cache.GetTeamsAsync();
    }

    public async Task<CollegeModel?> GetTeamByNameAsync(string name)
    {
        var teams = await _cache.GetTeamsAsync();
        return teams.FirstOrDefault(t =>
            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<CollegeModel>> SearchTeamsAsync(string query)
    {
        var teams = await _cache.GetTeamsAsync();
        return teams
            .Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<IReadOnlyList<ScheduleGame>> GetScheduleAsync()
    {
        return await _cache.GetScheduleAsync();
    }
}
