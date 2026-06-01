using System.Runtime.CompilerServices;
using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public abstract class SeasonEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public class SeasonStartedEvent : SeasonEvent
{
    public required int TotalGames { get; init; }
    public required int TotalWeeks { get; init; }
    public required List<CollegeModel> InitialRankings { get; init; }
}

public class SeasonDayCompletedEvent : SeasonEvent
{
    public required DateTime GameDate { get; init; }
    public required List<MatchupResult> Results { get; init; }
}

public class SeasonWeekCompletedEvent : SeasonEvent
{
    public required int WeekNumber { get; init; }
    public required List<CollegeModel> Rankings { get; init; }
}

public class SeasonCompletedEvent : SeasonEvent
{
    public required List<CollegeModel> FinalRankings { get; init; }
    public required List<MatchupResult> AllResults { get; init; }
}

public class SeasonEngine
{
    private readonly GameEngine _gameEngine;

    public SeasonEngine(SimulationConfig config, Random? random = null)
    {
        _gameEngine = new GameEngine(config, random);
    }

    public async IAsyncEnumerable<SeasonEvent> SimulateSeasonAsync(
        List<CollegeModel> teams,
        List<ScheduleGame> schedule,
        SimSpeed speed = SimSpeed.Medium,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        int delayMs = (int)speed;

        var clones = teams.Select(t => t.Clone()).ToList();
        foreach (var t in clones)
        {
            t.Wins = 0;
            t.Losses = 0;
            t.ConfWins = 0;
            t.ConfLosses = 0;
            t.CustomRankAdjuster = (decimal)(36.6 - (t.Rank / 10));
        }
        var lookup = clones.ToDictionary(t => t.Name);

        var sorted = schedule.OrderBy(s => s.GameDate).ToList();
        if (sorted.Count == 0) yield break;

        var firstDeadline = sorted[0].GameDate.Date;
        while (firstDeadline.DayOfWeek != DayOfWeek.Sunday)
            firstDeadline = firstDeadline.AddDays(1);

        int totalWeeks = 1;
        var scanDeadline = firstDeadline;
        foreach (var g in sorted)
        {
            while (g.GameDate > scanDeadline)
            {
                scanDeadline = scanDeadline.AddDays(7);
                totalWeeks++;
            }
        }

        int totalGames = sorted.Count(g =>
            lookup.ContainsKey(g.HomeTeam) && lookup.ContainsKey(g.AwayTeam));

        yield return new SeasonStartedEvent
        {
            TotalGames = totalGames,
            TotalWeeks = totalWeeks,
            InitialRankings = RerankTeams(clones).Take(25).ToList()
        };

        var gamesByDate = sorted.GroupBy(g => g.GameDate.Date).OrderBy(g => g.Key);
        var deadlineDate = firstDeadline;
        int weekNumber = 1;
        var allResults = new List<MatchupResult>();

        foreach (var dayGroup in gamesByDate)
        {
            ct.ThrowIfCancellationRequested();

            if (dayGroup.Key > deadlineDate)
            {
                var ranked = RerankTeams(clones);
                yield return new SeasonWeekCompletedEvent
                {
                    WeekNumber = weekNumber,
                    Rankings = ranked.Take(25).ToList()
                };
                deadlineDate = deadlineDate.AddDays(7);
                weekNumber++;

                while (dayGroup.Key > deadlineDate)
                {
                    deadlineDate = deadlineDate.AddDays(7);
                    weekNumber++;
                }
            }

            var dayResults = new List<MatchupResult>();

            foreach (var game in dayGroup)
            {
                if (!lookup.TryGetValue(game.AwayTeam, out var awayTeam) ||
                    !lookup.TryGetValue(game.HomeTeam, out var homeTeam))
                    continue;

                var result = await QuickSimAsync(awayTeam, homeTeam, ct);
                dayResults.Add(result);
                allResults.Add(result);

                var winner = lookup[result.Winner];
                var loser = lookup[result.Loser];
                winner.Wins++;
                loser.Losses++;

                decimal winnerAdj = (decimal)(36.6 - (loser.Rank / 10)) / 10;
                winner.CustomRankAdjuster += winnerAdj;
                decimal loserAdj = (decimal)(winner.Rank / 10) / 10;
                loser.CustomRankAdjuster -= loserAdj;

                if (game.ConferenceGame)
                {
                    winner.ConfWins++;
                    loser.ConfLosses++;
                }
            }

            if (dayResults.Count > 0)
            {
                yield return new SeasonDayCompletedEvent
                {
                    GameDate = dayGroup.Key,
                    Results = dayResults
                };

                if (delayMs > 0) await Task.Delay(delayMs, ct);
            }
        }

        var finalRanked = RerankTeams(clones);
        yield return new SeasonWeekCompletedEvent
        {
            WeekNumber = weekNumber,
            Rankings = finalRanked.Take(25).ToList()
        };

        yield return new SeasonCompletedEvent
        {
            FinalRankings = finalRanked.Take(100).ToList(),
            AllResults = allResults
        };
    }

    private static List<CollegeModel> RerankTeams(List<CollegeModel> teams)
    {
        var ranked = teams
            .OrderByDescending(t => t.CustomRankAdjuster)
            .ThenByDescending(t => t.BARTHAG)
            .ToList();
        for (int i = 0; i < ranked.Count; i++)
            ranked[i].Rank = i + 1;
        return ranked;
    }

    private async Task<MatchupResult> QuickSimAsync(
        CollegeModel away, CollegeModel home, CancellationToken ct)
    {
        await foreach (var evt in _gameEngine.SimulateGameAsync(away, home, SimSpeed.Instant, ct))
        {
            if (evt is GameCompletedEvent completed)
                return completed.Result;
        }
        throw new InvalidOperationException("Game simulation did not produce a result");
    }
}
