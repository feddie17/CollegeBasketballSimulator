using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public interface IGameEngine
{
    IAsyncEnumerable<GameEvent> SimulateGameAsync(
        CollegeModel away,
        CollegeModel home,
        SimSpeed speed = SimSpeed.Medium,
        CancellationToken ct = default);
}
