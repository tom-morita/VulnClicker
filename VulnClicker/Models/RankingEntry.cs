using Microsoft.Win32;

namespace VulnClicker.Models;

public class RankingEntry
{
    public int Rank { get; set; }
    public string Version { get; set; } = "";
    public DateTimeOffset Timestamp { get; set; }
    public string Player { get; set; } = "";
    public int Score { get; set; }
    public int Hit { get; set; }
    public int Miss { get; set; }
    public int Combo { get; set; }
}