using CBBSimulator.Data.Caching;

namespace CBBSimulator.Web.BackgroundServices;

public class DataRefreshService : BackgroundService
{
    private readonly TeamDataCache _cache;
    private readonly ILogger<DataRefreshService> _logger;
    private readonly TimeSpan _refreshInterval;

    public DataRefreshService(
        TeamDataCache cache,
        ILogger<DataRefreshService> logger,
        IConfiguration config)
    {
        _cache = cache;
        _logger = logger;
        _refreshInterval = TimeSpan.FromMinutes(
            config.GetValue("DATA_REFRESH_INTERVAL_MINUTES", 60));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial load on startup
        _logger.LogInformation("Performing initial data load...");
        await RefreshDataAsync(stoppingToken);
        _logger.LogInformation("Initial data load complete. {Count} teams loaded.",
            (await _cache.GetTeamsAsync()).Count);

        // Periodic refresh
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_refreshInterval, stoppingToken);

            _logger.LogInformation("Refreshing team data...");
            await RefreshDataAsync(stoppingToken);
            _logger.LogInformation("Data refresh complete.");
        }
    }

    private async Task RefreshDataAsync(CancellationToken ct)
    {
        try
        {
            await _cache.RefreshTeamsAsync(ct);
            await _cache.RefreshScheduleAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh data from CSV source");
        }
    }
}
