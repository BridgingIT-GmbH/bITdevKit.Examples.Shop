namespace Modules.Identity.Application;

using FluentValidation;

public class IdentityModuleConfiguration
{
    public AuthProvider Provider { get; set; } = AuthProvider.Jwt;

    public JwtConfiguration Jwt { get; set; } = new JwtConfiguration();

    public AzureAdConfiguration AzureAd { get; set; } = new AzureAdConfiguration();

    public IDictionary<string, string> ConnectionStrings { get; set; }

    public class Validator : AbstractValidator<IdentityModuleConfiguration>
    {
        public Validator()
        {
            this.RuleFor(c => c.Jwt.SigningSecret)
                .NotNull().NotEmpty()
                .WithMessage("SigningSecret cannot be null or empty");
        }
    }

    public class JwtConfiguration
    {
        public string SigningSecret { get; set; }

        public TimeSpan TokenLifetime { get; set; }

        public TimeSpan RefreshTokenLifetime { get; set; }
    }

    public class AzureAdConfiguration
    {
        public string Authority { get; set; }

        public string Instance { get; set; }

        public string Domain { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string Scopes { get; set; }

        public string ClientSecret { get; set; }

        public string CallbackPath { get; set; }

        public string SignedOutCallbackPath { get; set; }

        public string SignUpSignInPolicyId { get; set; }

        public string SignedOutRedirectUri { get; set; }
    }
}

public enum AuthProvider
{
    Jwt,
    AzureAd
}