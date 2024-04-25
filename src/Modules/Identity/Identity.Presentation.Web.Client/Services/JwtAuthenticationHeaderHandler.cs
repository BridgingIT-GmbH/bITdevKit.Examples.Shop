namespace Modules.Identity.Presentation.Web.Client;

using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;

public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ILogger<JwtAuthenticationHeaderHandler> logger;
    private readonly ILocalStorageService localStorage;

    public JwtAuthenticationHeaderHandler(
        ILogger<JwtAuthenticationHeaderHandler> logger,
        ILocalStorageService localStorage)
    {
        this.logger = logger;
        this.localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Scheme != "Bearer")
        {
            var token = await this.localStorage.GetItemAsync<string>(Common.StorageConstants.Local.AuthToken, cancellationToken);
            if (!string.IsNullOrWhiteSpace(token))
            {
                this.logger.LogInformation("auth: add bearer header");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
