namespace VulnClicker.Models;

public enum RuntimeProfile
{
    Standard,
    Diagnostic
}

public sealed class StartOptions
{
    public RuntimeProfile Profile { get; set; } = RuntimeProfile.Standard;

    public bool SoundEnabled { get; set; } = true;
    public bool AlwaysOnTop { get; set; } = false;

    public bool IsDebugMode => Profile == RuntimeProfile.Diagnostic;
}