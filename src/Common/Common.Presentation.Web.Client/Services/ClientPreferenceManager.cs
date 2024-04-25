namespace Common.Presentation.Web.Client;

using BridgingIT.DevKit.Common;
using Blazored.LocalStorage;
using Common;
using Common.Presentation.Web.Client.Models;
using Microsoft.Extensions.Localization;
using MudBlazor;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly ILocalStorageService storageService;
    private readonly IStringLocalizer<ClientPreferenceManager> localizer;

    public ClientPreferenceManager(
        ILocalStorageService localStorageService,
        IStringLocalizer<ClientPreferenceManager> localizer)
    {
        this.storageService = localStorageService;
        this.localizer = localizer;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            preference.IsDarkMode = !preference.IsDarkMode;
            await this.SetPreference(preference);
            return !preference.IsDarkMode;
        }

        return false;
    }

    public async Task<bool> ToggleLayoutDirection()
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            preference.IsRtl = !preference.IsRtl;
            await this.SetPreference(preference);
            return preference.IsRtl;
        }

        return false;
    }

    public async Task<Result> ChangeLanguageAsync(string languageCode)
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            preference.LanguageCode = languageCode;
            await this.SetPreference(preference);

            return Result.Success(new List<string> { this.localizer["Client language has been changed"] });
        }

        return Result.Failure(new List<string> { this.localizer["Failed to get client preferences"] });
    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            if (preference.IsDarkMode)
            {
                return new AppTheme().DarkTheme;
            }
        }

        return new AppTheme().DefaultTheme;
    }

    public async Task<bool> IsDarkMode()
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            return preference.IsDarkMode;
        }

        return false;
    }

    public async Task<bool> IsRtl()
    {
        if (await this.GetPreference() is ClientPreference preference)
        {
            return preference.IsRtl;
        }

        return false;
    }

    public async Task<IPreference> GetPreference()
    {
        return await this.storageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
    }

    public async Task SetPreference(IPreference preference)
    {
        await this.storageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
    }
}
