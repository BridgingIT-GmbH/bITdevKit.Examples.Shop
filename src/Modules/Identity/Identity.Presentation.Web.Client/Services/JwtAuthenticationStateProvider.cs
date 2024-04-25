namespace Modules.Identity.Presentation.Web.Client;

using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService localStorage;

    public JwtAuthenticationStateProvider(
        ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public ClaimsPrincipal AuthenticationStateUser { get; set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authToken = await this.localStorage.GetItemAsync<string>(Common.StorageConstants.Local.AuthToken);
        if (string.IsNullOrWhiteSpace(authToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var authState = new AuthenticationState(new ClaimsPrincipal(
            new ClaimsIdentity(this.GetClaimsFromJwt(authToken), "jwt")));
        this.AuthenticationStateUser = authState.User;
        return authState;
    }

    public async Task StateChangedAsync() =>
        this.NotifyAuthenticationStateChanged(Task.FromResult(await this.GetAuthenticationStateAsync()));

    public void SetNotAuthenticated() =>
        this.NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));

    //public async Task<ClaimsPrincipal> GetAuthenticationStateUserAsync() =>
    //    (await this.GetAuthenticationStateAsync()).User;

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = this.ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            keyValuePairs.TryGetValue("Permission", out var permissions);
            if (permissions != null)
            {
                if (permissions.ToString().Trim().StartsWith("["))
                {
                    var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                    claims.AddRange(parsedPermissions.Select(permission => new Claim("Permission", permission)));
                }
                else
                {
                    claims.Add(new Claim("Permission", permissions.ToString()));
                }

                keyValuePairs.Remove("Permission");
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
#pragma warning disable SA1407 // Arithmetic expressions should declare precedence
        var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
#pragma warning restore SA1407 // Arithmetic expressions should declare precedence
        return Convert.FromBase64String(base64);
    }
}