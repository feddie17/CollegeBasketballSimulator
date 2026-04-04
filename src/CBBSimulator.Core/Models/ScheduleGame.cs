namespace CBBSimulator.Core.Models;

public class ScheduleGame
{
    public string AwayTeam { get; set; } = "";
    public string HomeTeam { get; set; } = "";
    public DateTime GameDate { get; set; }
    public bool ConferenceGame { get; set; }
}
