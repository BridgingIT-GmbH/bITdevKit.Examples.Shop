namespace Presentation.Web;

using System.Globalization;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Localization;

public static class ApplicationBuilderExtensions // TODO: move to BridgingIT.DevKit (Presentation)
{
    /// <summary>
    /// Adds middleware to set the culture for each HTTP request.
    /// </summary>
    /// <param name="app"></param>
    public static IApplicationBuilder UseRequestCultureLocalization(this IApplicationBuilder app)
    {
        EnsureArg.IsNotNull(app, nameof(app));

        var supportedCultures = Common.LocalizationConstants.SupportedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();
        if (supportedCultures.SafeAny())
        {
            app.UseRequestLocalization(options =>
            {
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                options.ApplyCurrentCultureToResponseHeaders = true;
            });
            app.UseMiddleware<RequestCultureMiddleware>();
        }

        return app;
    }
}