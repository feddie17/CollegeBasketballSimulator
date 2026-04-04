using CBBSimulator.Core.Models;
using CBBSimulator.Data.Scrapers;
using Microsoft.Extensions.Caching.Memory;

namespace CBBSimulator.Data.Caching;

public class TeamDataCache
{
    private readonly IMemoryCache _cache;
    private readonly ITorkvikScraper _scraper;
    private readonly ICsvDataLoader _csvLoader;

    private const string TeamsCacheKey = "teams";
    private const string ScheduleCacheKey = "schedule";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public DateTime LastRefreshed { get; private set; }

    public TeamDataCache(IMemoryCache cache, ITorkvikScraper scraper, ICsvDataLoader csvLoader)
    {
        _cache = cache;
        _scraper = scraper;
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
        // TODO: Implement scraping + caching
        // 1. Scrape team data from Torvik
        // 2. Batch-fetch FT% for all teams
        // 3. Cache with TTL
        // 4. Update LastRefreshed
        var teams = new List<CollegeModel>();
        _cache.Set(TeamsCacheKey, teams, CacheDuration);
        LastRefreshed = DateTime.UtcNow;
        return teams;
    }

    public async Task<IReadOnlyList<ScheduleGame>> RefreshScheduleAsync(CancellationToken ct = default)
    {
        var schedule = new List<ScheduleGame>();
        _cache.Set(ScheduleCacheKey, schedule, CacheDuration);
        return schedule;
    }
}
