namespace Common.Presentation.Web.Client.Components;

using MudBlazor;

public partial class LanguageSelector
{
    private async Task ChangeLanguageAsync(string languageCode)
    {
        var result = await this.clientPreferenceManager.ChangeLanguageAsync(languageCode);
        this.snackbar.Add(this.localizer[result.Messages?.FirstOrDefault()], Severity.Success);
        await Task.Delay(700);
        this.navManager.NavigateTo(this.navManager.Uri, forceLoad: true);
    }
}
