using CBBSimulator.Core.Models;

namespace CBBSimulator.Data.Scrapers;

public interface ICsvDataLoader
{
    Task<List<CollegeModel>> LoadTeamDataAsync(string filePath, CancellationToken ct = default);
    Task<List<ScheduleGame>> LoadScheduleAsync(string filePath, CancellationToken ct = default);
}

public class CsvDataLoader : ICsvDataLoader
{
    public async Task<List<CollegeModel>> LoadTeamDataAsync(string filePath, CancellationToken ct = default)
    {
        // TODO: Port logic from Simulator2026.ScrapeData2026
        // 1. Read CSV file
        // 2. Parse columns into CollegeModel
        // 3. Sort by BARTHAG, assign ranks
        throw new NotImplementedException();
    }

    public async Task<List<ScheduleGame>> LoadScheduleAsync(string filePath, CancellationToken ct = default)
    {
        // TODO: Port logic from Simulator2026.Scrape2026Schedule
        // 1. Read CSV file
        // 2. Parse into ScheduleGame objects
        throw new NotImplementedException();
    }
}
