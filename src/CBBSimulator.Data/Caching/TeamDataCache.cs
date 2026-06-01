using CBBSimulator.Core.Models;
using CBBSimulator.Data.Scrapers;
using Microsoft.Extensions.Caching.Memory;

namespace CBBSimulator.Data.Caching;

public class TeamDataCache
{
    private readonly IMemoryCache _cache;
    private readonly ICsvDataLoader _csvLoader;

    private const string TeamsCacheKey = "teams";
    private const string ScheduleCacheKey = "schedule";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private const string TeamCsvName = "trank_data.csv";
    private const string ScheduleCsvName = "2026_super_sked.csv";
    private const string TeamResultsCsvName = "2026_team_results.csv";

    public DateTime LastRefreshed { get; private set; }

    public TeamDataCache(IMemoryCache cache, ICsvDataLoader csvLoader)
    {
        _cache = cache;
        _csvLoader = csvLoader;
    }

    public async Task<IReadOnlyList<CollegeModel>> GetTeamsAsync()
    {
        if (_cache.TryGetValue(TeamsCacheKey, out List<CollegeModel>? teams) && teams != null)
            return teams;

        return await RefreshTeamsAsync();
    }

    public async Task<IReadOnlyList<ScheduleGame>> GetScheduleAsync()
    {
        if (_cache.TryGetValue(ScheduleCacheKey, out List<ScheduleGame>? schedule) && schedule != null)
            return schedule;

        return await RefreshScheduleAsync();
    }

    public async Task<IReadOnlyList<CollegeModel>> RefreshTeamsAsync(CancellationToken ct = default)
    {
        var teamsPath = ResolveDataFilePath(TeamCsvName);
        var teams = await _csvLoader.LoadTeamDataAsync(teamsPath, ct);

        // The team stats CSV is a predictive projection with no conference,
        // record, or résumé data; enrich it from the team-results dataset.
        try
        {
            var resultsPath = ResolveDataFilePath(TeamResultsCsvName);
            var results = await _csvLoader.LoadTeamResultsAsync(resultsPath, ct);
            foreach (var team in teams)
            {
                if (results.TryGetValue(team.Name, out var info))
                {
                    team.Conference = info.Conference;
                    team.Record = info.Record;
                    team.Sos = info.Sos;
                    team.WAB = info.Wab; // résumé WAB (full season), used for tournament seeding
                }
            }
        }
        catch (FileNotFoundException)
        {
            // Results file is optional for enrichment; leave fields blank if absent.
        }

        _cache.Set(TeamsCacheKey, teams, CacheDuration);
        LastRefreshed = DateTime.UtcNow;
        return teams;
    }

    public async Task<IReadOnlyList<ScheduleGame>> RefreshScheduleAsync(CancellationToken ct = default)
    {
        var schedulePath = ResolveDataFilePath(ScheduleCsvName);
        var schedule = await _csvLoader.LoadScheduleAsync(schedulePath, ct);
        _cache.Set(ScheduleCacheKey, schedule, CacheDuration);
        return schedule;
    }

    private static string ResolveDataFilePath(string fileName)
    {
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        // Fallback for local runs where content hasn't been copied to bin/Data yet.
        var repoPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "src", "CBBSimulator.Data", "Data", fileName);
        var fullRepoPath = Path.GetFullPath(repoPath);
        if (File.Exists(fullRepoPath))
        {
            return fullRepoPath;
        }

        throw new FileNotFoundException($"CSV data file not found: {fileName}");
    }
}
