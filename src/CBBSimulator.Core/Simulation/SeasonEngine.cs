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
    public required int WeekNumber { get; init; }
    public required List<MatchupResult> Results { get; init; }
    public required List<CollegeModel> Standings { get; init; }
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

/// <summary>
/// A stateful, steppable season simulation. Unlike a one-shot streaming run, this
/// holds the season's state between calls so a caller can advance one day or one
/// week at a time, inspect the standings, and resume — enabling interactive
/// "sim a day / sim a week / stop" control from the UI.
/// </summary>
public class SeasonSimulation
{
    private const int StandingsSize = 100;

    private readonly GameEngine _gameEngine;
    private readonly List<CollegeModel> _clones;
    private readonly Dictionary<string, CollegeModel> _lookup;
    private readonly List<DaySlate> _days;
    private readonly List<MatchupResult> _allResults = new();
    private int _dayIndex;

    public int TotalGames { get; }
    public int TotalWeeks { get; }
    public int CurrentWeek { get; private set; }
    public bool IsComplete => _dayIndex >= _days.Count;

    private sealed record DaySlate(DateTime Date, int Week, List<ScheduleGame> Games);

    public SeasonSimulation(
        IEnumerable<CollegeModel> teams,
        IEnumerable<ScheduleGame> schedule,
        SimulationConfig config,
        Random? random = null)
    {
        _gameEngine = new GameEngine(config, random);

        _clones = teams.Select(t => t.Clone()).ToList();
        foreach (var t in _clones)
        {
            t.Wins = 0;
            t.Losses = 0;
            t.ConfWins = 0;
            t.ConfLosses = 0;
            t.CustomRankAdjuster = (decimal)(36.6 - (t.Rank / 10));
        }
        _lookup = _clones.ToDictionary(t => t.Name);

        var sorted = schedule.OrderBy(s => s.GameDate).ToList();
        _days = new List<DaySlate>();

        if (sorted.Count > 0)
        {
            // Weeks run Sunday-to-Sunday; tag each game day with its week number.
            var deadline = sorted[0].GameDate.Date;
            while (deadline.DayOfWeek != DayOfWeek.Sunday)
                deadline = deadline.AddDays(1);

            int week = 1;
            foreach (var dayGroup in sorted.GroupBy(g => g.GameDate.Date).OrderBy(g => g.Key))
            {
                while (dayGroup.Key > deadline)
                {
                    deadline = deadline.AddDays(7);
                    week++;
                }
                _days.Add(new DaySlate(dayGroup.Key, week, dayGroup.ToList()));
            }
        }

        TotalWeeks = _days.Count > 0 ? _days[^1].Week : 0;
        TotalGames = _days.Sum(d => d.Games.Count(g =>
            _lookup.ContainsKey(g.HomeTeam) && _lookup.ContainsKey(g.AwayTeam)));
        CurrentWeek = _days.Count > 0 ? _days[0].Week : 0;
    }

    public SeasonStartedEvent BuildStartedEvent() => new()
    {
        TotalGames = TotalGames,
        TotalWeeks = TotalWeeks,
        InitialRankings = CurrentStandings()
    };

    public List<CollegeModel> CurrentStandings() => RerankTeams(_clones).Take(StandingsSize).ToList();

    /// <summary>
    /// Simulates the next scheduled day. Emits a day-completed event (with the
    /// updated standings); a week-completed event when this was the week's last
    /// day; and a season-completed event when the schedule is exhausted.
    /// </summary>
    public async Task<List<SeasonEvent>> SimulateNextDayAsync(CancellationToken ct = default)
    {
        var events = new List<SeasonEvent>();
        if (IsComplete)
        {
            return events;
        }

        var slate = _days[_dayIndex];
        CurrentWeek = slate.Week;
        var dayResults = new List<MatchupResult>();

        foreach (var game in slate.Games)
        {
            ct.ThrowIfCancellationRequested();

            if (!_lookup.TryGetValue(game.AwayTeam, out var awayTeam) ||
                !_lookup.TryGetValue(game.HomeTeam, out var homeTeam))
                continue;

            var result = await QuickSimAsync(awayTeam, homeTeam, ct);
            dayResults.Add(result);
            _allResults.Add(result);

            var winner = _lookup[result.Winner];
            var loser = _lookup[result.Loser];
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

        _dayIndex++;
        var standings = CurrentStandings();

        events.Add(new SeasonDayCompletedEvent
        {
            GameDate = slate.Date,
            WeekNumber = slate.Week,
            Results = dayResults,
            Standings = standings
        });

        bool weekEnded = IsComplete || _days[_dayIndex].Week != slate.Week;
        if (weekEnded)
        {
            events.Add(new SeasonWeekCompletedEvent
            {
                WeekNumber = slate.Week,
                Rankings = standings
            });
        }

        if (IsComplete)
        {
            events.Add(new SeasonCompletedEvent
            {
                FinalRankings = standings,
                AllResults = _allResults.ToList()
            });
        }

        return events;
    }

    /// <summary>
    /// Simulates every remaining day in the current week (or to the end of the
    /// season, whichever comes first), aggregating the emitted events in order.
    /// </summary>
    public async Task<List<SeasonEvent>> SimulateNextWeekAsync(CancellationToken ct = default)
    {
        var events = new List<SeasonEvent>();
        if (IsComplete)
        {
            return events;
        }

        int targetWeek = _days[_dayIndex].Week;
        while (!IsComplete && _days[_dayIndex].Week == targetWeek)
        {
            events.AddRange(await SimulateNextDayAsync(ct));
        }

        return events;
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
