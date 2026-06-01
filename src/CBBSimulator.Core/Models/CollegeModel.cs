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

    public CollegeModel Clone() => new CollegeModel
    {
        Rank = Rank, Name = Name, Conference = Conference,
        GamesPlayed = GamesPlayed, Wins = Wins, Losses = Losses,
        ConfWins = ConfWins, ConfLosses = ConfLosses,
        ADJOE = ADJOE, ADJDE = ADJDE, BARTHAG = BARTHAG,
        EFG_O = EFG_O, EFG_D = EFG_D, TOR_O = TOR_O, TOR_D = TOR_D,
        ORB = ORB, DRB = DRB, FTR_O = FTR_O, FTR_D = FTR_D,
        PT2_O = PT2_O, PT2_D = PT2_D, PT3_O = PT3_O, PT3_D = PT3_D,
        ADJ_T = ADJ_T, WAB = WAB, FTP = FTP,
        CustomRankAdjuster = CustomRankAdjuster
    };
}
