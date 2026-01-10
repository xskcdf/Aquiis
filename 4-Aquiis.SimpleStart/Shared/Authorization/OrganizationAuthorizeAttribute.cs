using Microsoft.AspNetCore.Authorization;

namespace Aquiis.SimpleStart.Shared.Authorization;

/// <summary>
/// Authorization attribute for organization-based role checking.
/// Replaces [Authorize(Roles = "...")] with organization-scoped roles.
/// When used without roles, allows any authenticated organization member.
/// </summary>
public class OrganizationAuthorizeAttribute : AuthorizeAttribute
{
    public OrganizationAuthorizeAttribute(params string[] roles)
    {
        if (roles == null || roles.Length == 0)
        {
            Policy = "OrganizationMember";
        }
        else
        {
            Policy = $"OrganizationRole:{string.Join(",", roles)}";
        }
    }
}
