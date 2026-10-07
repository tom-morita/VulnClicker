
using Microsoft.Win32;

namespace VulnClicker.Models;

public class TitleTheme

{
    public string Title
    {
        get;
        set;
    } = "";

    public Color BackColor
    {
        get;
        set;
    }

    public Color TitleColor
    {
        get;
        set;
    }

    public Color ButtonBackColor
    {
        get;
        set;
    }

    public Color ButtonForeColor
    {
        get;
        set;
    }
}