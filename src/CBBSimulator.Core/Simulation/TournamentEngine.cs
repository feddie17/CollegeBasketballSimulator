using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public class TournamentGameInfo
{
    public required string AwayTeam { get; init; }
    public required string HomeTeam { get; init; }
    public required string Round { get; init; }
    public int GameNumber { get; init; }
}

public abstract class TournamentEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public class TournamentGameStartedEvent : TournamentEvent
{
    public required TournamentGameInfo GameInfo { get; init; }
}

public class TournamentGameCompletedEvent : TournamentEvent
{
    public required MatchupResult Result { get; init; }
    public required string Round { get; init; }
}

public class TournamentRoundCompletedEvent : TournamentEvent
{
    public required string Round { get; init; }
    public required List<MatchupResult> Results { get; init; }
}

public class TournamentCompletedEvent : TournamentEvent
{
    public required string Champion { get; init; }
    public required List<MatchupResult> AllResults { get; init; }
}

public class TournamentEngine
{
    private readonly GameEngine _gameEngine;

    public TournamentEngine(SimulationConfig config, Random? random = null)
    {
        _gameEngine = new GameEngine(config, random);
    }

    public async IAsyncEnumerable<TournamentEvent> SimulateTournamentAsync(
        List<CollegeModel> teams,
        SimSpeed speed = SimSpeed.Medium,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        // TODO: Port logic from MarchMadnessController
        // 1. Build bracket from seeded team list
        // 2. For each round, simulate all matchups
        // 3. Yield TournamentGameStartedEvent before each game
        // 4. Yield TournamentGameCompletedEvent after each game
        // 5. Yield TournamentRoundCompletedEvent after each round
        // 6. Yield TournamentCompletedEvent when champion decided

        yield return new TournamentCompletedEvent
        {
            Champion = teams.First().Name,
            AllResults = new List<MatchupResult>()
        };

        await Task.CompletedTask;
    }
}
