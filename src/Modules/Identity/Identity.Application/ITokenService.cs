namespace Modules.Identity.Application;

using BridgingIT.DevKit.Common;

public interface ITokenService
{
    Task<Result<TokenModel>> GetTokenAsync(string email, string password);

    Task<Result<TokenModel>> GetRefreshTokenAsync(string token, string refreshToken);
}