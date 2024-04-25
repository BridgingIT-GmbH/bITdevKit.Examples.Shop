namespace Modules.Identity.Presentation;

using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        this.FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => this.FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        //if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
        if (!policyName.IsNullOrEmpty())
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new PermissionRequirement(policyName));

            return Task.FromResult<AuthorizationPolicy>(policy.Build());
        }

        return this.FallbackPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => Task.FromResult<AuthorizationPolicy>(null);
}