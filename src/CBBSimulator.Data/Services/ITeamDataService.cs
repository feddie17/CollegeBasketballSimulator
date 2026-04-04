using CBBSimulator.Core.Models;

namespace CBBSimulator.Data.Services;

public interface ITeamDataService
{
    Task<IReadOnlyList<CollegeModel>> GetAllTeamsAsync();
    Task<CollegeModel?> GetTeamByNameAsync(string name);
    Task<IReadOnlyList<CollegeModel>> SearchTeamsAsync(string query);
    Task<IReadOnlyList<ScheduleGame>> GetScheduleAsync();
    DateTime LastRefreshed { get; }
}
