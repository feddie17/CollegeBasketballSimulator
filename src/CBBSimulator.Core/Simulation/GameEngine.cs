using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public class GameEngine : IGameEngine
{
    private readonly PossessionEngine _possessionEngine;

    public GameEngine(SimulationConfig config, Random? random = null)
    {
        _possessionEngine = new PossessionEngine(config, random);
    }

    public async IAsyncEnumerable<GameEvent> SimulateGameAsync(
        CollegeModel away,
        CollegeModel home,
        SimSpeed speed = SimSpeed.Medium,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var state = CreateInitialState(away, home);
        int delayMs = (int)speed;

        yield return new PeriodStartedEvent
        {
            Period = state.PeriodLabel,
            Scoreboard = state.ToScoreboardState()
        };

        await foreach (var evt in SimulateUntilClockAsync(
            state, away, home, 1200, useLateGameLogic: false, delayMs, ct))
        {
            yield return evt;
        }

        yield return new PeriodEndedEvent
        {
            Period = "1st Half",
            Scoreboard = state.ToScoreboardState()
        };

        state.ResetFouls();
        state.StartSecondHalf();

        yield return new HalftimeEvent
        {
            Scoreboard = state.ToScoreboardState()
        };

        yield return new PeriodStartedEvent
        {
            Period = state.PeriodLabel,
            Scoreboard = state.ToScoreboardState()
        };

        await foreach (var evt in SimulateUntilClockAsync(
            state, away, home, 120, useLateGameLogic: false, delayMs, ct))
        {
            yield return evt;
        }

        await foreach (var evt in SimulateUntilClockAsync(
            state, away, home, 0, useLateGameLogic: true, delayMs, ct))
        {
            yield return evt;
        }

        yield return new PeriodEndedEvent
        {
            Period = "2nd Half",
            Scoreboard = state.ToScoreboardState()
        };

        int overtimeCounter = 0;
        while (state.AwayScore == state.HomeScore)
        {
            ct.ThrowIfCancellationRequested();
            overtimeCounter++;

            state.StartOvertime(overtimeCounter);
            yield return new OvertimeStartedEvent
            {
                OvertimeNumber = overtimeCounter,
                Scoreboard = state.ToScoreboardState()
            };
            yield return new PeriodStartedEvent
            {
                Period = state.PeriodLabel,
                Scoreboard = state.ToScoreboardState()
            };

            await foreach (var evt in SimulateUntilClockAsync(
                state, away, home, 120, useLateGameLogic: false, delayMs, ct))
            {
                yield return evt;
            }

            await foreach (var evt in SimulateUntilClockAsync(
                state, away, home, 0, useLateGameLogic: true, delayMs, ct))
            {
                yield return evt;
            }

            yield return new PeriodEndedEvent
            {
                Period = state.PeriodLabel,
                Scoreboard = state.ToScoreboardState()
            };
        }

        yield return new GameCompletedEvent
        {
            Result = BuildFinalResult(state)
        };
    }

    private static GameSimulationState CreateInitialState(CollegeModel away, CollegeModel home)
    {
        var state = new GameSimulationState
        {
            AwayTeam = away.Name,
            HomeTeam = home.Name
        };
        state.SetInitialPossession(homeHasPossession: false);
        return state;
    }

    private async IAsyncEnumerable<GameEvent> SimulateUntilClockAsync(
        GameSimulationState state,
        CollegeModel away,
        CollegeModel home,
        int stopAtSeconds,
        bool useLateGameLogic,
        int delayMs,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        while (state.SecondsRemaining > stopAtSeconds)
        {
            ct.ThrowIfCancellationRequested();

            if (useLateGameLogic)
            {
                int lead = Math.Abs(state.AwayScore - state.HomeScore);
                if (state.SecondsRemaining < 30 && lead > 6)
                {
                    break;
                }
            }

            var previousClock = state.ToScoreboardState().Clock;
            var possession = RunPossessionStep(state, away, home, useLateGameLogic);
            state.AdvanceClock(possession.SecondsUsed);
            state.ApplyPossession(state.HomeHasPossession, possession);

            var offenseTeam = state.HomeHasPossession ? home.Name : away.Name;
            var defenseTeam = state.HomeHasPossession ? away.Name : home.Name;
            var currentBoard = state.ToScoreboardState();

            yield return new ClockAdvancedEvent
            {
                PreviousClock = previousClock,
                CurrentClock = currentBoard.Clock,
                SecondsElapsed = possession.SecondsUsed,
                Period = state.PeriodLabel,
                Scoreboard = currentBoard
            };

            yield return new PossessionEvent
            {
                Team = offenseTeam,
                Action = DescribePossession(possession),
                PointsScored = possession.PointsScored,
                OffenseTeam = offenseTeam,
                DefenseTeam = defenseTeam,
                DefensiveFoul = possession.DefensiveFoul,
                OffensiveFoul = possession.OffensiveFoul,
                SecondsUsed = possession.SecondsUsed,
                Scoreboard = currentBoard
            };

            yield return new ScoreUpdatedEvent
            {
                Scoreboard = currentBoard,
                PointsScored = possession.PointsScored,
                ScoringTeam = possession.PointsScored > 0 ? offenseTeam : string.Empty
            };

            if (!possession.OffenseKeepsPossession)
            {
                state.TogglePossession();
            }

            if (delayMs > 0)
            {
                await Task.Delay(delayMs, ct);
            }
        }
    }

    private PossessionResult RunPossessionStep(
        GameSimulationState state,
        CollegeModel away,
        CollegeModel home,
        bool useLateGameLogic)
    {
        bool offenseIsHome = state.HomeHasPossession;
        var offense = offenseIsHome ? home : away;
        var defense = offenseIsHome ? away : home;
        int scoreDiff = state.ScoreDifferentialForOffense(offenseIsHome);
        int defensiveFouls = state.DefensiveFouls(offenseIsHome);
        bool shortPossession = useLateGameLogic;

        return _possessionEngine.Run(
            offense,
            defense,
            scoreDiff,
            defensiveFouls,
            shortPossession,
            offenseIsHome);
    }

    private static MatchupResult BuildFinalResult(GameSimulationState state)
    {
        var result = new MatchupResult
        {
            AwayTeam = state.AwayTeam,
            HomeTeam = state.HomeTeam,
            AwayTeamScore = state.AwayScore,
            HomeTeamScore = state.HomeScore,
            Overtimes = state.OvertimeCount
        };

        if (state.AwayScore > state.HomeScore)
        {
            result.Winner = state.AwayTeam;
            result.WinnerScore = state.AwayScore;
            result.Loser = state.HomeTeam;
            result.LoserScore = state.HomeScore;
        }
        else
        {
            result.Winner = state.HomeTeam;
            result.WinnerScore = state.HomeScore;
            result.Loser = state.AwayTeam;
            result.LoserScore = state.AwayScore;
        }

        return result;
    }

    private static string DescribePossession(PossessionResult possession)
    {
        if (possession.OffensiveFoul)
        {
            return "Offensive foul";
        }

        if (possession.DefensiveFoul && possession.PointsScored > 0)
        {
            return $"Shooting foul, {possession.PointsScored} points";
        }

        if (possession.DefensiveFoul)
        {
            return "Defensive foul";
        }

        return possession.PointsScored switch
        {
            3 => "Made 3PT",
            2 => "Made 2PT",
            1 => "Made free throw",
            _ => "Empty possession"
        };
    }
}
