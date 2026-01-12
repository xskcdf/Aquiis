using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.Professional.Shared.Components.Account;
using Aquiis.Infrastructure.Data;
using Aquiis.Core.Constants;
using System.Security.Claims;
using Aquiis.Professional.Entities;

namespace Aquiis.Professional.Shared.Authorization;

/// <summary>
/// Authorization handler for organization role requirements.
/// Checks if the user has the required role in their active organization.
/// </summary>
public class OrganizationRoleAuthorizationHandler : AuthorizationHandler<OrganizationRoleRequirement>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationRoleAuthorizationHandler(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrganizationRoleRequirement requirement)
    {
        // User must be authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        // Get user ID from claims
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // Get user's active organization
        var user = await _userManager.FindByIdAsync(userId);
        if (user?.ActiveOrganizationId == null)
        {
            return;
        }

        // Get user's role in the active organization
        var userOrganization = await _dbContext.UserOrganizations
            .Where(uo => uo.UserId == userId 
                      && uo.OrganizationId == user.ActiveOrganizationId 
                      && uo.IsActive 
                      && !uo.IsDeleted)
            .FirstOrDefaultAsync();

        if (userOrganization == null)
        {
            return;
        }

        // Check if user's role is in the allowed roles
        // If no roles specified (empty array), allow any authenticated org member
        if (requirement.AllowedRoles.Length == 0 || requirement.AllowedRoles.Contains(userOrganization.Role))
        {
            context.Succeed(requirement);
        }
    }
}
