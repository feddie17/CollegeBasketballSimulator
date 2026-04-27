using CBBSimulator.Core.Models;
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
        _logger.LogInformation("Performing initial data load...");
        var teams = await RefreshDataAsync(stoppingToken);
        _logger.LogInformation(
            "Initial data load complete. {Count} teams loaded ({FtpCount} with FT% data).",
            teams?.Count ?? 0, teams?.Count(t => t.FTP > 0) ?? 0);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_refreshInterval, stoppingToken);

            _logger.LogInformation("Refreshing team data...");
            teams = await RefreshDataAsync(stoppingToken);
            _logger.LogInformation(
                "Data refresh complete. {Count} teams loaded.",
                teams?.Count ?? 0);
        }
    }

    private async Task<IReadOnlyList<CollegeModel>?> RefreshDataAsync(CancellationToken ct)
    {
        try
        {
            var teams = await _cache.RefreshTeamsAsync(ct);
            await _cache.RefreshScheduleAsync(ct);
            return teams;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh data from CSV source");
            return null;
        }
    }
}
