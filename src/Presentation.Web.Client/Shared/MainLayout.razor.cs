namespace Presentation.Web.Client.Shared;

using Common.Presentation.Web.Client;
using MudBlazor;

public partial class MainLayout
{
    private MudTheme currentTheme = new AppTheme().DefaultTheme;
    private bool isDarkMode;
    private bool rightToLeft;

    protected override async Task OnInitializedAsync()
    {
        this.currentTheme = await this.clientPreferenceManager.GetCurrentThemeAsync();
        this.isDarkMode = await this.clientPreferenceManager.IsDarkMode();
        this.rightToLeft = await this.clientPreferenceManager.IsRtl();
    }

    private async Task RightToLeftToggle(bool value)
    {
        this.rightToLeft = value;
        await Task.CompletedTask;
    }

    private async Task DarkModeToggle()
    {
        await this.clientPreferenceManager.ToggleDarkModeAsync();
        this.isDarkMode = await this.clientPreferenceManager.IsDarkMode();
        this.currentTheme = this.isDarkMode
            ? new AppTheme().DarkTheme
            : new AppTheme().DefaultTheme;
    }
}
