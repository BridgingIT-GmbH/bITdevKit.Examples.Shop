namespace Modules.Identity.Presentation.Web.Client;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

public class AzureAdClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    // Can't work with actual services in the constructor here, have to
    // use IServiceProvider, otherwise the app hangs at startup.
    // The culprit is probably HttpClient, as this class is instantiated
    // at startup while the HttpClient is being (or not even) created.
    private readonly IServiceProvider services;

    public AzureAdClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, IServiceProvider services)
        : base(accessor) =>
        this.services = services;

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var principal = await base.CreateUserAsync(account, options);

        if (principal.Identity?.IsAuthenticated is true)
        {
            Console.WriteLine("====== AzureAdClaimsPrincipalFactory ====");

            var profile = await this.services.GetRequiredService<IIdentityApiClient>().Identity_AccountGetProfileAsync();
            var identity = (ClaimsIdentity)principal.Identity;

            if (!string.IsNullOrWhiteSpace(profile.Value.Email) && !identity.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, profile.Value.Email));
            }

            if (!string.IsNullOrWhiteSpace(profile.Value.PhoneNumber) && !identity.HasClaim(c => c.Type == ClaimTypes.MobilePhone))
            {
                identity.AddClaim(new Claim(ClaimTypes.MobilePhone, profile.Value.PhoneNumber));
            }

            if (!string.IsNullOrWhiteSpace(profile.Value.FirstName) && !identity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, profile.Value.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(profile.Value.LastName) && !identity.HasClaim(c => c.Type == ClaimTypes.Surname))
            {
                identity.AddClaim(new Claim(ClaimTypes.Surname, profile.Value.LastName));
            }

            if (!identity.HasClaim(c => c.Type == "fullname"))
            {
                identity.AddClaim(new Claim("fullname", $"{profile.Value.FirstName} {profile.Value.LastName}"));
            }

            if (!identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, profile.Value.Id));
            }

            if (!string.IsNullOrWhiteSpace(profile.Value.ImageUrl) && !identity.HasClaim(c => c.Type == "imageurl") && profile.Value.ImageUrl is not null)
            {
                identity.AddClaim(new Claim("imageurl", profile.Value.ImageUrl));
            }

            if (profile.Value.Permissions != null)
            {
                foreach (var permission in profile.Value.Permissions)
                {
                    Console.WriteLine($"====== AzureAdClaimsPrincipalFactory: ADD PERMISSION TO IDENTITY: {permission}");
                    identity.AddClaim(new Claim("Permission", permission));
                }
            }
        }

        return principal;
    }
}