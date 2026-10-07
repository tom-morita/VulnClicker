namespace VulnClicker.Models;

public class GameSaveData
{
    public string Player { get; set; } = "";

    public int Score { get; set; }

    public int Hit { get; set; }

    public int Miss { get; set; }

    public int Combo { get; set; }
    public int RemainingTime { get; set; }
}