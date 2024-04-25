namespace Common;

using BridgingIT.DevKit.Common;

public interface IAuthenticationManager
{
    void NavigateToExternalLogin(string returnUrl);

    Task<Result> LoginAsync(string email, string password);

    Task<Result> LogoutAsync();

    Task<string> GetToken();

    Task<string> RefreshTokenAsync();

    Task<string> TryRefreshTokenAsync();

    Task<string> TryForceRefreshTokenAsync();

    Task<bool> IsTokenValidAsync();

    Task<DateTimeOffset> GetTokenExpirationAsync();
}