namespace Presentation.Web.Client;

using System.Globalization;
using Common.Presentation.Web.Client;
using Common.Presentation.Web.Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

public static class WebAssemblyHostExtensions
{
    public static async Task<WebAssemblyHost> UseLocalization(
        this WebAssemblyHost source,
        string defaultLanguage)
    {
        defaultLanguage ??= "en-US";
        var storageService = source.Services.GetRequiredService<IClientPreferenceManager>();
        if (storageService != null)
        {
            var culture = (await storageService.GetPreference() is ClientPreference preference)
                ? new CultureInfo(preference.LanguageCode ?? defaultLanguage)
                : new CultureInfo(defaultLanguage);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        return source;
    }
}
