namespace Modules.Identity.Presentation;

using System.Security.Claims;

public static class ClaimsPrincipalExtension
{
    public static string GetIssuer(this ClaimsPrincipal principal)
    {
        if (principal.FindFirstValue("iss") is string issuer)
        {
            return issuer;
        }

        // Workaround to deal with missing "iss" claim. We search for the ObjectId claim instead and return the value of Issuer property of that Claim
        return principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Issuer;
    }
}
