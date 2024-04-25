namespace Modules.Identity.Application;

public class TokenModel
{
    public string Token { get; set; }

    public string RefreshToken { get; set; }

    public string UserImageURL { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; } // TODO: needed by client?
}
