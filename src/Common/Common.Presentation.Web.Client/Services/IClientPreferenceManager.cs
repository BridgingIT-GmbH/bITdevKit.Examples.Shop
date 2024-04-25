namespace Common.Presentation.Web.Client;

using BridgingIT.DevKit.Common;
using Common.Presentation.Web.Client.Models;
using MudBlazor;

public interface IClientPreferenceManager : IPreferenceManager
{
    Task<MudTheme> GetCurrentThemeAsync();

    Task<bool> ToggleDarkModeAsync();

    Task<bool> ToggleLayoutDirection();

    Task<bool> IsDarkMode();

    Task<bool> IsRtl();
}

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<Result> ChangeLanguageAsync(string languageCode);
}
