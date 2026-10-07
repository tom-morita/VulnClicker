namespace VulnClickerScoreServer.Models;

public class ScoreEntry
{
    public string Player { get; set; } = "";
    public int Score { get; set; }
    public int Hit { get; set; }
    public int Miss { get; set; }
    public int Combo { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Version { get; set; } = "";
}