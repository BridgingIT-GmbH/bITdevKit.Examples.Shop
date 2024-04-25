namespace Common.Presentation.Web.Client.Models;

public class AppConfiguration
{
    public bool EnableLanguageToggle { get; set; } = true;

    public bool EnableDarkModeToggle { get; set; } = true;

    public bool TableHover { get; set; } = true;

    public bool TableDense { get; set; }

    public bool TableStriped { get; set; } = true;

    public bool TableBordered { get; set; }

    public Dictionary<string, object> Settings { get; set; } = new();
}