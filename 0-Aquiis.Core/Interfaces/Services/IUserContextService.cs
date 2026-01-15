using Aquiis.Core.Entities;

namespace Aquiis.Core.Interfaces.Services;

/// <summary>
/// Service interface for accessing user context information.
/// Implementations are project-specific and depend on ApplicationDbContext.
/// </summary>
public interface IUserContextService
{
    /// <summary>
    /// Gets the current authenticated user's ID.
    /// </summary>
    Task<string?> GetUserIdAsync();

    /// <summary>
    /// Gets the current user's active organization ID.
    /// </summary>
    Task<Guid?> GetActiveOrganizationIdAsync();

    /// <summary>
    /// Gets the current authenticated user's full name.
    /// </summary>
    Task<string?> GetUserNameAsync();

    /// <summary>
    /// Gets the current user's email address.
    /// </summary>
    Task<string?> GetUserEmailAsync();

    /// <summary>
    /// Gets the current user's OrganizationId (DEPRECATED: Use GetActiveOrganizationIdAsync).
    /// </summary>
    Task<Guid?> GetOrganizationIdAsync();

    /// <summary>
    /// Forces a refresh of the cached user data.
    /// Call this if user data has been updated and you need to reload it.
    /// </summary>
    Task RefreshAsync();

    Task<Organization?> GetActiveOrganizationAsync();

    Task<string?> GetCurrentOrganizationRoleAsync();
       
    Task<bool> IsAccountOwnerAsync();
   

    /// <summary>
    /// Switch the user's active organization
    /// </summary>
    Task<bool> SwitchOrganizationAsync(Guid organizationId);

    /// <summary>
    /// Check if the current user has a specific permission in their active organization
    /// </summary>
    Task<bool> HasPermissionAsync(string permission);
}