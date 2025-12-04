using Microsoft.AspNetCore.Authorization;

namespace Aquiis.SimpleStart.Shared.Authorization;

/// <summary>
/// Authorization attribute for organization-based role checking.
/// Replaces [Authorize(Roles = "...")] with organization-scoped roles.
/// </summary>
public class OrganizationAuthorizeAttribute : AuthorizeAttribute
{
    public OrganizationAuthorizeAttribute(params string[] roles)
    {
        Policy = $"OrganizationRole:{string.Join(",", roles)}";
    }
}
