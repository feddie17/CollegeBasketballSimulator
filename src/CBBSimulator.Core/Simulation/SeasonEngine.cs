using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public abstract class SeasonEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public class SeasonGameCompletedEvent : SeasonEvent
{
    public required MatchupResult Result { get; init; }
    public required DateTime GameDate { get; init; }
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
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        // TODO: Port logic from Simulator2026.SimulateFullSchedule
        // 1. Sort schedule by date
        // 2. Group games by week
        // 3. Simulate each game, update CustomRankAdjuster
        // 4. Yield SeasonGameCompletedEvent after each game
        // 5. Yield SeasonWeekCompletedEvent with updated rankings after each week
        // 6. Yield SeasonCompletedEvent when done

        yield return new SeasonCompletedEvent
        {
            FinalRankings = teams,
            AllResults = new List<MatchupResult>()
        };

        await Task.CompletedTask;
    }
}
