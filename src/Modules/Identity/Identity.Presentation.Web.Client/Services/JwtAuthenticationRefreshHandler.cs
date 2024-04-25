namespace Modules.Identity.Presentation.Web.Client;

using System.Security;
using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

public class JwtAuthenticationRefreshHandler : DelegatingHandler
{
    private readonly ILogger<JwtAuthenticationRefreshHandler> logger;
    private readonly IAuthenticationManager authManager;
    private readonly NavigationManager navManager;

    public JwtAuthenticationRefreshHandler(
        ILogger<JwtAuthenticationRefreshHandler> logger,
        IAuthenticationManager authManager,
        NavigationManager navManager)
    {
        this.logger = logger;
        this.authManager = authManager;
        this.navManager = navManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        if (!await this.authManager.IsTokenValidAsync())
        {
            this.logger.LogWarning("auth: token invalid > refresh");
            try
            {
                await this.authManager.RefreshTokenAsync();
            }
            catch (SecurityException)
            {
                // token could not be refreshed (expired?)
                this.logger.LogWarning("auth: refresh token failed > logout");

                await this.authManager.LogoutAsync();
                this.navManager.NavigateTo($"{IdentityPageConstants.Login}?redirectUrl={Uri.EscapeDataString(this.navManager.Uri)}"/*, forceLoad: true*/);
                cts.Cancel(); // cancels the request
            }
        }
        else
        {
            this.logger.LogInformation("auth: token valid");
        }

        return await base.SendAsync(request, cts.Token);
    }
}
