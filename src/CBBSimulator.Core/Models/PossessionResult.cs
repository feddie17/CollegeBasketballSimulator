namespace CBBSimulator.Core.Models;

public class PossessionResult
{
    public int PointsScored { get; set; }
    public int SecondsUsed { get; set; }
    public bool OffenseKeepsPossession { get; set; }
    public bool DefensiveFoul { get; set; }
    public bool OffensiveFoul { get; set; }
}
