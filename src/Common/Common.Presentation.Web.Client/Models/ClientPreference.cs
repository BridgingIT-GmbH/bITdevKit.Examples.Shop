namespace Common.Presentation.Web.Client.Models;

public class ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; }

    public bool IsRtl { get; set; }

    public bool IsDrawerOpen { get; set; }

    public string PrimaryColor { get; set; }

    public string LanguageCode { get; set; }
}
