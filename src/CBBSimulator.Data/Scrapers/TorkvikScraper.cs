using CBBSimulator.Core.Models;
using HtmlAgilityPack;

namespace CBBSimulator.Data.Scrapers;

public interface ITorkvikScraper
{
    Task<List<CollegeModel>> ScrapeTeamDataAsync(CancellationToken ct = default);
    Task<decimal> ScrapeFreeThrowPercentageAsync(string teamName, CancellationToken ct = default);
}

public class TorkvikScraper : ITorkvikScraper
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://barttorvik.com";

    public TorkvikScraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CollegeModel>> ScrapeTeamDataAsync(CancellationToken ct = default)
    {
        // TODO: Port logic from DataController.ScrapeLive2025Data
        // 1. Fetch HTML from barttorvik.com
        // 2. Parse table rows with HtmlAgilityPack
        // 3. Map columns to CollegeModel properties
        throw new NotImplementedException();
    }

    public async Task<decimal> ScrapeFreeThrowPercentageAsync(string teamName, CancellationToken ct = default)
    {
        // TODO: Port logic from DataController.UpdateFreeThrow
        // 1. Fetch team page from barttorvik.com/team.php?team={name}
        // 2. Extract FT% from element with id "ft_per"
        // 3. Default to 70% if not found
        throw new NotImplementedException();
    }
}
