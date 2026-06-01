using CBBSimulator.Core.Models;
using System.Globalization;

namespace CBBSimulator.Data.Scrapers;

public interface ICsvDataLoader
{
    Task<List<CollegeModel>> LoadTeamDataAsync(string filePath, CancellationToken ct = default);
    Task<List<ScheduleGame>> LoadScheduleAsync(string filePath, CancellationToken ct = default);
    Task<Dictionary<string, TeamResultInfo>> LoadTeamResultsAsync(string filePath, CancellationToken ct = default);
}

/// <summary>
/// Season-results enrichment for a team, sourced from the team-results dataset:
/// conference, real W-L record, strength of schedule, and wins above bubble.
/// </summary>
public record TeamResultInfo(string Conference, string Record, decimal Sos, decimal Wab);

public class CsvDataLoader : ICsvDataLoader
{
    public async Task<List<CollegeModel>> LoadTeamDataAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV not found at: {filePath}", filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath, ct);
        var teams = new List<CollegeModel>();

        foreach (var line in lines.Skip(1))
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = ParseCsvLine(line);
            if (columns.Count <= 35)
            {
                continue;
            }

            if (!TryParseTeam(columns, out var team))
            {
                continue;
            }

            teams.Add(team);
        }

        teams = teams.OrderByDescending(x => x.BARTHAG).ToList();

        var rank = 1;
        foreach (var team in teams)
        {
            team.Rank = rank;
            team.CustomRankAdjuster = 36.6m - (team.Rank / 10m);
            rank++;
        }

        return teams;
    }

    public async Task<List<ScheduleGame>> LoadScheduleAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV not found at: {filePath}", filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath, ct);
        var schedule = new List<ScheduleGame>();

        foreach (var line in lines)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = ParseCsvLine(line);
            if (columns.Count <= 15)
            {
                continue;
            }

            if (!TryParseScheduleGame(columns, out var game))
            {
                continue;
            }

            schedule.Add(game);
        }

        return schedule.OrderBy(x => x.GameDate).ToList();
    }

    /// <summary>
    /// Reads per-team season-results enrichment (conference, record, SOS, WAB)
    /// directly from the team-results dataset (a header-rowed CSV). Columns are
    /// located by header name so the parse is resilient to column reordering.
    /// </summary>
    public async Task<Dictionary<string, TeamResultInfo>> LoadTeamResultsAsync(string filePath, CancellationToken ct = default)
    {
        var result = new Dictionary<string, TeamResultInfo>(StringComparer.Ordinal);
        if (!File.Exists(filePath))
        {
            return result;
        }

        var lines = await File.ReadAllLinesAsync(filePath, ct);
        if (lines.Length < 2)
        {
            return result;
        }

        var header = ParseCsvLine(lines[0]);
        var teamIndex = FindColumnIndex(header, "team");
        var confIndex = FindColumnIndex(header, "conf");
        var recordIndex = FindColumnIndex(header, "record");
        var sosIndex = FindColumnIndex(header, "sos");
        var wabIndex = FindColumnIndex(header, "WAB");
        if (teamIndex < 0)
        {
            return result;
        }

        var maxIndex = new[] { teamIndex, confIndex, recordIndex, sosIndex, wabIndex }.Max();

        foreach (var line in lines.Skip(1))
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = ParseCsvLine(line);
            if (columns.Count <= maxIndex)
            {
                continue;
            }

            var team = columns[teamIndex].Trim().Replace("\"", "");
            if (string.IsNullOrWhiteSpace(team))
            {
                continue;
            }

            var conf = confIndex >= 0 ? columns[confIndex].Trim().Replace("\"", "") : "";
            var record = recordIndex >= 0 ? columns[recordIndex].Trim().Replace("\"", "") : "";
            TryParseDecimal(sosIndex >= 0 ? columns[sosIndex] : "", out var sos);
            TryParseDecimal(wabIndex >= 0 ? columns[wabIndex] : "", out var wab);

            result[team] = new TeamResultInfo(conf, record, sos, wab);
        }

        return result;
    }

    private static int FindColumnIndex(IReadOnlyList<string> header, string name)
    {
        for (var i = 0; i < header.Count; i++)
        {
            if (header[i].Trim().Replace("\"", "").Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }
        return -1;
    }

    private static bool TryParseTeam(IReadOnlyList<string> columns, out CollegeModel team)
    {
        team = new CollegeModel();
        if (!TryParseDecimal(columns[1], out var adjoe) ||
            !TryParseDecimal(columns[2], out var adjde) ||
            !TryParseDecimal(columns[3], out var barthag) ||
            !TryParseDecimal(columns[7], out var efgO) ||
            !TryParseDecimal(columns[8], out var efgD) ||
            !TryParseDecimal(columns[11], out var torO) ||
            !TryParseDecimal(columns[12], out var torD) ||
            !TryParseDecimal(columns[13], out var orb) ||
            !TryParseDecimal(columns[14], out var drb) ||
            !TryParseDecimal(columns[9], out var ftrO) ||
            !TryParseDecimal(columns[10], out var ftrD) ||
            !TryParseDecimal(columns[16], out var pt2O) ||
            !TryParseDecimal(columns[17], out var pt2D) ||
            !TryParseDecimal(columns[18], out var pt3O) ||
            !TryParseDecimal(columns[19], out var pt3D) ||
            !TryParseDecimal(columns[15], out var adjT) ||
            !TryParseDecimal(columns[34], out var wab))
        {
            return false;
        }

        var ftp = TryParseDecimal(columns[35], out var ftpValue) && ftpValue > 0m ? ftpValue : 70m;

        team = new CollegeModel
        {
            Rank = 0,
            CustomRankAdjuster = 0m,
            Name = columns[0].Trim().Replace("\"", ""),
            Conference = "",
            GamesPlayed = 0,
            Wins = 0,
            Losses = 0,
            ADJOE = adjoe,
            ADJDE = adjde,
            BARTHAG = barthag,
            EFG_O = efgO,
            EFG_D = efgD,
            TOR_O = torO,
            TOR_D = torD,
            ORB = orb,
            DRB = drb,
            FTR_O = ftrO,
            FTR_D = ftrD,
            PT2_O = pt2O,
            PT2_D = pt2D,
            PT3_O = pt3O,
            PT3_D = pt3D,
            ADJ_T = adjT,
            WAB = wab,
            FTP = ftp
        };

        return !string.IsNullOrWhiteSpace(team.Name);
    }

    private static bool TryParseScheduleGame(IReadOnlyList<string> columns, out ScheduleGame game)
    {
        game = new ScheduleGame();
        if (!DateTime.TryParse(columns[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return false;
        }

        var awayTeam = columns[8].Trim().Replace("\"", "");
        var homeTeam = columns[14].Trim().Replace("\"", "");
        if (string.IsNullOrWhiteSpace(awayTeam) || string.IsNullOrWhiteSpace(homeTeam))
        {
            return false;
        }

        game = new ScheduleGame
        {
            AwayTeam = awayTeam,
            HomeTeam = homeTeam,
            GameDate = date,
            ConferenceGame = IsConferenceGame(columns[2])
        };

        return true;
    }

    private static bool IsConferenceGame(string confColumnRaw)
        => TryParseConferenceMatchup(confColumnRaw, out var away, out var home)
           && away.Equals(home, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Parses a matchup descriptor like "SB at MAC" or "ACC vs. ACC" into the
    /// away and home conference codes. Splits on the spaced separators (" at ",
    /// " vs. ") so codes that themselves contain "at" (e.g. "Pat") parse correctly.
    /// </summary>
    private static bool TryParseConferenceMatchup(string raw, out string away, out string home)
    {
        away = string.Empty;
        home = string.Empty;
        var value = raw.Trim();
        string[] parts;

        if (value.Contains(" vs. ", StringComparison.Ordinal))
        {
            parts = value.Split(" vs. ", StringSplitOptions.TrimEntries);
        }
        else if (value.Contains(" at ", StringComparison.Ordinal))
        {
            parts = value.Split(" at ", StringSplitOptions.TrimEntries);
        }
        else
        {
            return false;
        }

        if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
        {
            return false;
        }

        away = parts[0];
        home = parts[1];
        return true;
    }

    private static bool TryParseDecimal(string value, out decimal result)
    {
        var normalized = value.Trim().Replace("\"", "").Replace("%", "");
        return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result;
    }
}
