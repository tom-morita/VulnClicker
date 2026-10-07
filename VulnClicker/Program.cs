
using VulnClicker.Models;
using VulnClicker.Forms;

namespace VulnClicker;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        StartOptions startOptions = ParseStartOptions(args);
        ApplicationConfiguration.Initialize();
        Application.Run(new TitleForm(startOptions));
    }

    private static StartOptions ParseStartOptions(
        string[] args)
    {
        RuntimeProfile profile = RuntimeProfile.Standard;
        bool soundEnabled = true;
        bool alwaysOnTop = false;

        foreach (string arg in args)
        {
            switch (arg.ToLowerInvariant())
            {
                case "--debug":
                    profile = RuntimeProfile.Diagnostic;
                    break;

                case "--nosound":
                    soundEnabled = false;
                    break;

                case "--ontop":
                    alwaysOnTop = true;
                    break;
           }
        }

        return new StartOptions
        {
            Profile = profile,
            SoundEnabled = soundEnabled,
            AlwaysOnTop = alwaysOnTop
        };
    }
}