using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Shared.Authorization;

/// <summary>
/// Custom authorization policy provider for organization roles.
/// Dynamically creates policies based on the OrganizationRole: prefix.
/// </summary>
public class OrganizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;
    private const string POLICY_PREFIX = "OrganizationRole:";

    public OrganizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName == "OrganizationMember")
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new OrganizationRoleRequirement(Array.Empty<string>()));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
        
        if (policyName.StartsWith(POLICY_PREFIX))
        {
            var roles = policyName.Substring(POLICY_PREFIX.Length).Split(',');
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new OrganizationRoleRequirement(roles));
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}
