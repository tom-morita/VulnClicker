
using Microsoft.Win32;

namespace VulnClicker.Models;

public class StartUpInfo
{
    public DateTime Today { get; set; }
    public DateTime StartUpDate { get; set; }
    public int StartUpCount { get; set; }
    public bool IsFirstStartUp { get; set; }
}