namespace CBBSimulator.Core.Models;

public class MatchupResult
{
    public string AwayTeam { get; set; } = "";
    public string HomeTeam { get; set; } = "";
    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public string Winner { get; set; } = "";
    public string Loser { get; set; } = "";
    public int WinnerScore { get; set; }
    public int LoserScore { get; set; }
    public int Overtimes { get; set; }
}
