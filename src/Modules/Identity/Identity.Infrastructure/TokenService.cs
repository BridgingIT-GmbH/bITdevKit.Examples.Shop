namespace Modules.Identity.Infrastructure;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.Identity.Application;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> logger;
    private readonly UserManager<AppIdentityUser> userManager;
    private readonly RoleManager<AppIdentityRole> roleManager;
    private readonly IStringLocalizer<TokenService> localizer;
    private readonly IdentityModuleConfiguration configuration;

    public TokenService(
        ILogger<TokenService> logger,
        UserManager<AppIdentityUser> userManager,
        RoleManager<AppIdentityRole> roleManager,
        IStringLocalizer<TokenService> localizer,
        IOptions<IdentityModuleConfiguration> configuration)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.localizer = localizer;
        this.configuration = configuration.Value;
    }

    public async Task<Result<TokenModel>> GetTokenAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return Result<TokenModel>.Failure("Invalid token request.");
        }

        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<TokenModel>.Failure("User not found.");
        }

        if (!user.IsActive)
        {
            return Result<TokenModel>.Failure("User not active.");
        }

        if (!user.EmailConfirmed)
        {
            return Result<TokenModel>.Failure("E-Mail not confirmed.");
        }

        if (!await this.userManager.CheckPasswordAsync(user, password))
        {
            return Result<TokenModel>.Failure("Invalid credentials.");
        }

        user.RefreshToken = this.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(this.configuration.Jwt.RefreshTokenLifetime);
        this.logger.LogDebug($"auth: Refresh token created (exp={user.RefreshTokenExpiryTime:o})");
        await this.userManager.UpdateAsync(user);

        return Result<TokenModel>.Success(
            new TokenModel
            {
                Token = await this.GenerateJwtAsync(user),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
                UserImageURL = user.ImageUrl
            });
    }

    public async Task<Result<TokenModel>> GetRefreshTokenAsync(string token, string refreshToken)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
        {
            this.logger.LogWarning("auth: Invalid token request");
            return Result<TokenModel>.Failure("Invalid token request.");
        }

        var userPrincipal = this.GetTokenPrincipal(token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await this.userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            this.logger.LogWarning("auth: User not found");
            return Result<TokenModel>.Failure("User not found.");
        }

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            this.logger.LogWarning($"auth: Invalid client refresh token {user.RefreshTokenExpiryTime} <= {DateTime.UtcNow}");
            return Result<TokenModel>.Failure("Invalid client refresh token.");
        }

        var resultToken = this.GenerateToken(this.GetSigningCredentials(), await this.GetClaimsAsync(user));
        user.RefreshToken = this.GenerateRefreshToken();
        await this.userManager.UpdateAsync(user);

        this.logger.LogDebug($"auth: Token refreshed (refresh exp={user.RefreshTokenExpiryTime:o})");
        return Result<TokenModel>.Success(new TokenModel
        {
            Token = resultToken,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        });
    }

    private async Task<string> GenerateJwtAsync(AppIdentityUser user)
    {
        return this.GenerateToken(this.GetSigningCredentials(), await this.GetClaimsAsync(user));
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(AppIdentityUser user)
    {
        var userRoleNames = await this.userManager.GetRolesAsync(user);
        var userClaims = await this.userManager.GetClaimsAsync(user);
        var roleClaims = new List<Claim>();
        var permClaims = new List<Claim>();

        if (userRoleNames?.Any() == true)
        {
            foreach (var roleName in userRoleNames)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roleName));
                var role = await this.roleManager.FindByNameAsync(roleName);
                var rolePermissions = await this.roleManager.GetClaimsAsync(role);
                permClaims.AddRange(rolePermissions);
            }
        }

        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permClaims);
    }

    private string GenerateToken(SigningCredentials credentials, IEnumerable<Claim> claims)
    {
        this.logger.LogDebug($"auth: Generate token (token exp={DateTime.UtcNow.Add(this.configuration.Jwt.TokenLifetime):o})");

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.Add(this.configuration.Jwt.TokenLifetime),
               signingCredentials: credentials));
    }

    private ClaimsPrincipal GetTokenPrincipal(string token)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.Jwt.SigningSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this.configuration.Jwt.SigningSecret)),
            SecurityAlgorithms.HmacSha256);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}