namespace CBBSimulator.Core.Models;

public enum SimSpeed
{
    Instant = 0,
    Fast = 50,
    Medium = 500,
    Slow = 2000
}

public abstract class GameEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public class PossessionEvent : GameEvent
{
    public required string Team { get; init; }
    public required string Action { get; init; }
    public int PointsScored { get; init; }
    public string OffenseTeam { get; init; } = "";
    public string DefenseTeam { get; init; } = "";
    public bool DefensiveFoul { get; init; }
    public bool OffensiveFoul { get; init; }
    public int SecondsUsed { get; init; }
    public required ScoreboardState Scoreboard { get; init; }
}

public class HalftimeEvent : GameEvent
{
    public required ScoreboardState Scoreboard { get; init; }
}

public class GameCompletedEvent : GameEvent
{
    public required MatchupResult Result { get; init; }
}

public class PeriodStartedEvent : GameEvent
{
    public required string Period { get; init; }
    public required ScoreboardState Scoreboard { get; init; }
}

public class ClockAdvancedEvent : GameEvent
{
    public required string PreviousClock { get; init; }
    public required string CurrentClock { get; init; }
    public int SecondsElapsed { get; init; }
    public required string Period { get; init; }
    public required ScoreboardState Scoreboard { get; init; }
}

public class ScoreUpdatedEvent : GameEvent
{
    public required ScoreboardState Scoreboard { get; init; }
    public int PointsScored { get; init; }
    public string ScoringTeam { get; init; } = "";
}

public class PeriodEndedEvent : GameEvent
{
    public required string Period { get; init; }
    public required ScoreboardState Scoreboard { get; init; }
}

public class OvertimeStartedEvent : GameEvent
{
    public int OvertimeNumber { get; init; }
    public required ScoreboardState Scoreboard { get; init; }
}

public class ScoreboardState
{
    public string AwayTeam { get; init; } = "";
    public string HomeTeam { get; init; } = "";
    public int AwayScore { get; init; }
    public int HomeScore { get; init; }
    public int AwayFouls { get; init; }
    public int HomeFouls { get; init; }
    public string Clock { get; init; } = "20:00";
    public string Period { get; init; } = "1st Half";
    public bool HomeHasPossession { get; init; }
}
