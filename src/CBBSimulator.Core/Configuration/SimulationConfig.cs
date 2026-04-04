namespace CBBSimulator.Core.Configuration;

public class SimulationConfig
{
    public int Adjuster2PT { get; set; } = -12;
    public int Adjuster3PT { get; set; } = -12;
    public int AdjusterTurnover { get; set; } = 0;
    public int AdjusterFoul { get; set; } = -5;
    public int AdjusterHomeTeam { get; set; } = 3;
    public bool NeutralSite { get; set; } = false;
}
