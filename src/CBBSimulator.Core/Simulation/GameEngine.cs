using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public class GameEngine : IGameEngine
{
    private readonly SimulationConfig _config;
    private readonly PossessionEngine _possessionEngine;

    public GameEngine(SimulationConfig config, Random? random = null)
    {
        _config = config;
        _possessionEngine = new PossessionEngine(config, random);
    }

    public async IAsyncEnumerable<GameEvent> SimulateGameAsync(
        CollegeModel away,
        CollegeModel home,
        SimSpeed speed = SimSpeed.Medium,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        // TODO: Port logic from DataController.RunMatchup
        // 1. Initialize game state (scores, fouls, time = 2400)
        // 2. First half loop (2400 -> 1200)
        // 3. Yield HalftimeEvent, reset fouls
        // 4. Second half loop (1200 -> 0)
        // 5. Overtime if tied
        // 6. Yield GameCompletedEvent

        yield return new GameCompletedEvent
        {
            Result = new MatchupResult
            {
                AwayTeam = away.Name,
                HomeTeam = home.Name
            }
        };

        await Task.CompletedTask;
    }
}
