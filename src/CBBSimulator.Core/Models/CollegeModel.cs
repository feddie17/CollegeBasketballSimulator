namespace CBBSimulator.Core.Models;

public class CollegeModel
{
    public int Rank { get; set; }
    public string Name { get; set; } = "";
    public string Conference { get; set; } = "";
    public int GamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int ConfWins { get; set; }
    public int ConfLosses { get; set; }
    public decimal ADJOE { get; set; }
    public decimal ADJDE { get; set; }
    public decimal BARTHAG { get; set; }
    public decimal EFG_O { get; set; }
    public decimal EFG_D { get; set; }
    public decimal TOR_O { get; set; }
    public decimal TOR_D { get; set; }
    public decimal ORB { get; set; }
    public decimal DRB { get; set; }
    public decimal FTR_O { get; set; }
    public decimal FTR_D { get; set; }
    public decimal PT2_O { get; set; }
    public decimal PT2_D { get; set; }
    public decimal PT3_O { get; set; }
    public decimal PT3_D { get; set; }
    public decimal ADJ_T { get; set; }
    public decimal WAB { get; set; }
    public decimal FTP { get; set; }
    public decimal CustomRankAdjuster { get; set; }
}
