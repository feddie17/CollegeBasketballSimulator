using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;
using CBBSimulator.Core.Simulation;

namespace CBBSimulator.Core.Tests;

public class GameEngineTests
{
    private static CollegeModel CreateTestTeam(string name, int rank = 1) => new()
    {
        Name = name,
        Rank = rank,
        Conference = "Test",
        ADJOE = 110m,
        ADJDE = 95m,
        BARTHAG = 0.9m,
        EFG_O = 52m,
        EFG_D = 48m,
        TOR_O = 18m,
        TOR_D = 20m,
        ORB = 30m,
        DRB = 70m,
        FTR_O = 35m,
        FTR_D = 30m,
        PT2_O = 0.50m,
        PT2_D = 0.48m,
        PT3_O = 0.35m,
        PT3_D = 0.33m,
        ADJ_T = 68m,
        FTP = 0.72m
    };

    [Fact]
    public async Task SimulateGameAsync_ReturnsGameCompletedEvent()
    {
        var config = new SimulationConfig();
        var engine = new GameEngine(config, new Random(42));

        var events = new List<GameEvent>();
        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            events.Add(evt);
        }

        Assert.Contains(events, e => e is GameCompletedEvent);
    }

    [Fact]
    public void SimulationConfig_HasCorrectDefaults()
    {
        var config = new SimulationConfig();

        Assert.Equal(-12, config.Adjuster2PT);
        Assert.Equal(-12, config.Adjuster3PT);
        Assert.Equal(-5, config.AdjusterFoul);
        Assert.Equal(3, config.AdjusterHomeTeam);
    }
}

public class PossessionEngineTests
{
    [Fact]
    public void Run_WithSeededRandom_IsDeterministic()
    {
        // TODO: Implement after PossessionEngine.Run is ported
        Assert.True(true, "Placeholder — implement after engine port");
    }
}
