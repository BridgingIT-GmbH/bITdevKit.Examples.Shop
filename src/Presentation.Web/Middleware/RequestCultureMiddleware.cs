namespace Presentation.Web;

using System.Globalization;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

public class RequestCultureMiddleware // TODO: move to BridgingIT.DevKit (Presentation)
{
    private const string CultureNameKey = "Culture";
    private readonly ILogger<RequestCultureMiddleware> logger;
    private readonly RequestDelegate next;

    public RequestCultureMiddleware(
        ILogger<RequestCultureMiddleware> logger,
        RequestDelegate next)
    {
        EnsureArg.IsNotNull(logger, nameof(logger));
        EnsureArg.IsNotNull(next, nameof(next));

        this.logger = logger;
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        EnsureArg.IsNotNull(httpContext, nameof(httpContext));

        if (GetPath(httpContext)
            .EqualsPatternAny(new[] { "/*.js", "/*.css", "/*.map", "/*.html", "/swagger*", "/favicon.ico", "/_framework*", "/notificationhub*", "/_content*", "/signalrhub*" }))
        {
            await this.next(httpContext); // continue pipeline
        }
        else
        {
            var cultureName = httpContext.Request.Query["culture"];
            this.SetRequestCulture(httpContext, cultureName);
            SetResponseHeaders(httpContext);

            if (!string.IsNullOrEmpty(cultureName))
            {
                using (this.logger.BeginScope(new Dictionary<string, object>
                {
                    [CultureNameKey] = cultureName,
                }))
                {
                    await this.next(httpContext); // continue pipeline
                }
            }
            else
            {
                await this.next(httpContext); // continue pipeline
            }
        }
    }

    private static string GetPath(HttpContext httpContext, bool includeQuery = false)
    {
        var path = includeQuery
            ? httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget
            : httpContext.Features.Get<IHttpRequestFeature>()?.Path;

        if (string.IsNullOrEmpty(path))
        {
            path = httpContext.Request.Path.ToString();
        }

        return path;
    }

    private static void SetResponseHeaders(HttpContext httpContext)
    {
        if (httpContext.Response.Headers.ContainsKey("Content-Language"))
        {
            httpContext.Response.Headers.Remove("Content-Language");
        }

        httpContext.Response.Headers.TryAdd("Content-Language", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
    }

    private void SetRequestCulture(HttpContext httpContext, Microsoft.Extensions.Primitives.StringValues cultureName)
    {
        if (!string.IsNullOrWhiteSpace(cultureName))
        {
            var culture = new CultureInfo(cultureName);

            this.logger.LogInformation("{LogKey} request culture: {RequestCulture}", "REQ", culture.TwoLetterISOLanguageName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        else if (httpContext.Request.Headers.ContainsKey("Accept-Language"))
        {
            var cultureHeader = httpContext.Request.Headers["Accept-Language"];
            if (cultureHeader.Count > 0)
            {
                var culture = new CultureInfo(cultureHeader[0].Split(',')[0].Trim());

                this.logger.LogInformation("{LogKey} culture: {RequestCulture}", "REQ", culture.TwoLetterISOLanguageName);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }
    }
}
