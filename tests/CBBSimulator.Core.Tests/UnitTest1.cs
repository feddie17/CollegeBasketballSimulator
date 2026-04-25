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
        PT2_O = 50m,
        PT2_D = 48m,
        PT3_O = 35m,
        PT3_D = 33m,
        ADJ_T = 68m,
        FTP = 72m
    };

    [Fact]
    public async Task SimulateGameAsync_EmitsGameCompletedEvent_Last()
    {
        var config = new SimulationConfig();
        var engine = new GameEngine(config, new Random(42));

        var events = new List<GameEvent>();
        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            events.Add(evt);
        }

        var final = Assert.IsType<GameCompletedEvent>(events[^1]);
        Assert.False(string.IsNullOrWhiteSpace(final.Result.Winner));
        Assert.False(string.IsNullOrWhiteSpace(final.Result.Loser));
    }

    [Fact]
    public async Task SimulateGameAsync_EmitsPeriodAndScoreEvents_InExpectedOrder()
    {
        var config = new SimulationConfig();
        var engine = new GameEngine(config, new Random(42));

        var events = new List<GameEvent>();
        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            events.Add(evt);
        }

        Assert.IsType<PeriodStartedEvent>(events[0]);

        var firstClockIndex = events.FindIndex(e => e is ClockAdvancedEvent);
        var firstPossessionIndex = events.FindIndex(e => e is PossessionEvent);
        var firstScoreIndex = events.FindIndex(e => e is ScoreUpdatedEvent);

        Assert.True(firstClockIndex >= 0);
        Assert.True(firstPossessionIndex > firstClockIndex);
        Assert.True(firstScoreIndex > firstPossessionIndex);
    }

    [Fact]
    public async Task SimulateGameAsync_WithSeededRandom_IsDeterministic()
    {
        var config = new SimulationConfig();
        var away = CreateTestTeam("Away");
        var home = CreateTestTeam("Home");

        var engineA = new GameEngine(config, new Random(1234));
        var engineB = new GameEngine(config, new Random(1234));

        MatchupResult? resultA = null;
        await foreach (var evt in engineA.SimulateGameAsync(away, home, SimSpeed.Instant))
        {
            if (evt is GameCompletedEvent done)
            {
                resultA = done.Result;
            }
        }

        MatchupResult? resultB = null;
        await foreach (var evt in engineB.SimulateGameAsync(away, home, SimSpeed.Instant))
        {
            if (evt is GameCompletedEvent done)
            {
                resultB = done.Result;
            }
        }

        Assert.NotNull(resultA);
        Assert.NotNull(resultB);
        Assert.Equal(resultA!.AwayTeamScore, resultB!.AwayTeamScore);
        Assert.Equal(resultA.HomeTeamScore, resultB.HomeTeamScore);
        Assert.Equal(resultA.Winner, resultB.Winner);
        Assert.Equal(resultA.Overtimes, resultB.Overtimes);
    }

    [Fact]
    public async Task SimulateGameAsync_ResetsFoulsAtHalftime()
    {
        var config = new SimulationConfig();
        var engine = new GameEngine(config, new Random(42));

        HalftimeEvent? halftime = null;
        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            if (evt is HalftimeEvent h)
            {
                halftime = h;
            }
        }

        Assert.NotNull(halftime);
        Assert.Equal(0, halftime!.Scoreboard.AwayFouls);
        Assert.Equal(0, halftime.Scoreboard.HomeFouls);
        Assert.Equal("2nd Half", halftime.Scoreboard.Period);
    }

    [Fact]
    public async Task SimulateGameAsync_ResultMatchesLastScoreUpdate()
    {
        var config = new SimulationConfig();
        var engine = new GameEngine(config, new Random(42));

        ScoreUpdatedEvent? lastScore = null;
        MatchupResult? final = null;

        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            if (evt is ScoreUpdatedEvent scoreUpdated)
            {
                lastScore = scoreUpdated;
            }

            if (evt is GameCompletedEvent completed)
            {
                final = completed.Result;
            }
        }

        Assert.NotNull(lastScore);
        Assert.NotNull(final);
        Assert.Equal(lastScore!.Scoreboard.AwayScore, final!.AwayTeamScore);
        Assert.Equal(lastScore.Scoreboard.HomeScore, final.HomeTeamScore);
    }

    [Fact]
    public async Task SimulateGameAsync_EmitsOvertimeStartedEvent_WhenTiedAfterRegulation()
    {
        var config = new SimulationConfig
        {
            Adjuster2PT = -90,
            Adjuster3PT = -90,
            AdjusterFoul = -50
        };
        var engine = new GameEngine(config, new Random(4683));

        var sawOvertime = false;
        await foreach (var evt in engine.SimulateGameAsync(
            CreateTestTeam("Away"), CreateTestTeam("Home"), SimSpeed.Instant))
        {
            if (evt is OvertimeStartedEvent)
            {
                sawOvertime = true;
                break;
            }
        }

        Assert.True(sawOvertime);
    }

    [Fact]
    public void SimulationConfig_Defaults_AreStable()
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
        var config = new SimulationConfig();
        var offense = GameEngineTestsAccessor.CreateTestTeam("Offense");
        var defense = GameEngineTestsAccessor.CreateTestTeam("Defense");

        var engineA = new PossessionEngine(config, new Random(99));
        var engineB = new PossessionEngine(config, new Random(99));

        var resultA = engineA.Run(offense, defense, 0, 0, false, true);
        var resultB = engineB.Run(offense, defense, 0, 0, false, true);

        Assert.Equal(resultA.PointsScored, resultB.PointsScored);
        Assert.Equal(resultA.SecondsUsed, resultB.SecondsUsed);
        Assert.Equal(resultA.OffenseKeepsPossession, resultB.OffenseKeepsPossession);
        Assert.Equal(resultA.DefensiveFoul, resultB.DefensiveFoul);
        Assert.Equal(resultA.OffensiveFoul, resultB.OffensiveFoul);
    }
}

internal static class GameEngineTestsAccessor
{
    internal static CollegeModel CreateTestTeam(string name, int rank = 1) => new()
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
        PT2_O = 50m,
        PT2_D = 48m,
        PT3_O = 35m,
        PT3_D = 33m,
        ADJ_T = 68m,
        FTP = 72m
    };
}
