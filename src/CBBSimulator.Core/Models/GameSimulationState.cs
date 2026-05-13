namespace CBBSimulator.Core.Models;

public class GameSimulationState
{
    public required string AwayTeam { get; init; }
    public required string HomeTeam { get; init; }

    public int AwayScore { get; private set; }
    public int HomeScore { get; private set; }
    public int AwayFouls { get; private set; }
    public int HomeFouls { get; private set; }

    public int SecondsRemaining { get; private set; } = 1200;
    public string PeriodLabel { get; private set; } = "1st Half";
    public int OvertimeCount { get; private set; }
    public bool HomeHasPossession { get; private set; }

    public void SetInitialPossession(bool homeHasPossession) => HomeHasPossession = homeHasPossession;

    public void AdvanceClock(int secondsUsed)
    {
        SecondsRemaining -= secondsUsed;
        if (SecondsRemaining < 0)
        {
            SecondsRemaining = 0;
        }
    }

    public void TogglePossession() => HomeHasPossession = !HomeHasPossession;

    public void ResetFouls()
    {
        AwayFouls = 0;
        HomeFouls = 0;
    }

    public void StartSecondHalf()
    {
        PeriodLabel = "2nd Half";
        SecondsRemaining = 1200;
    }

    public void StartOvertime(int overtimeNumber)
    {
        OvertimeCount = overtimeNumber;
        PeriodLabel = $"OT{overtimeNumber}";
        SecondsRemaining = 300;
    }

    public void ApplyPossession(bool offenseIsHome, PossessionResult possession)
    {
        if (offenseIsHome)
        {
            HomeScore += possession.PointsScored;
            if (possession.OffensiveFoul)
            {
                HomeFouls++;
            }

            if (possession.DefensiveFoul)
            {
                AwayFouls++;
            }
        }
        else
        {
            AwayScore += possession.PointsScored;
            if (possession.OffensiveFoul)
            {
                AwayFouls++;
            }

            if (possession.DefensiveFoul)
            {
                HomeFouls++;
            }
        }
    }

    public int OffensiveTeamScore(bool offenseIsHome) => offenseIsHome ? HomeScore : AwayScore;

    public int DefensiveTeamScore(bool offenseIsHome) => offenseIsHome ? AwayScore : HomeScore;

    public int DefensiveFouls(bool offenseIsHome) => offenseIsHome ? AwayFouls : HomeFouls;

    public int ScoreDifferentialForOffense(bool offenseIsHome)
    {
        return OffensiveTeamScore(offenseIsHome) - DefensiveTeamScore(offenseIsHome);
    }

    public ScoreboardState ToScoreboardState()
    {
        return new ScoreboardState
        {
            AwayTeam = AwayTeam,
            HomeTeam = HomeTeam,
            AwayScore = AwayScore,
            HomeScore = HomeScore,
            AwayFouls = AwayFouls,
            HomeFouls = HomeFouls,
            Clock = FormatClock(SecondsRemaining),
            Period = PeriodLabel,
            HomeHasPossession = HomeHasPossession
        };
    }

    private static string FormatClock(int secondsRemaining)
    {
        int minutes = secondsRemaining / 60;
        int seconds = secondsRemaining % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}
