namespace Modules.Identity.Presentation.Web.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;

public class AzureAdAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public AzureAdAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration)
        : base(provider, navigation)
    {
        // TODO: check config values to be not null!
        this.ConfigureHandler(
                new[] { configuration["App:ApiBaseUrl"] },
                new[] { configuration["App:Modules:Identity:AzureAd:ApiScope"] });
    }
}
