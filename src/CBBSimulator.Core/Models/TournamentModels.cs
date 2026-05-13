namespace CBBSimulator.Core.Models;

public class TournamentBracket
{
    public required List<TournamentRegion> Regions { get; init; }
    public List<PlayInMatchup> PlayInGames { get; init; } = new();
}

public class TournamentRegion
{
    public required string Name { get; init; }
    public List<TournamentSeed> Seeds { get; init; } = new();
}

public class TournamentSeed
{
    public int Seed { get; init; }
    public required string TeamName { get; init; }
}

public class PlayInMatchup
{
    public required string Team1 { get; init; }
    public required string Team2 { get; init; }
    public required string TargetRegion { get; init; }
    public int TargetSeed { get; init; }
}
