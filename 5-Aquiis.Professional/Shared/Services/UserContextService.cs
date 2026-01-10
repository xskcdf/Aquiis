using Aquiis.Professional.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Aquiis.Core.Entities;
using System.Security.Claims;
using Aquiis.Core.Constants;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application.Services;

namespace Aquiis.Professional.Shared.Services
{

    /// <summary>
    /// Provides cached access to the current user's context information including OrganizationId.
    /// This service is scoped per Blazor circuit, so the data is cached for the user's session.
    /// </summary>
    public class UserContextService : IUserContextService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Func<Task<Aquiis.Application.Services.OrganizationService>> _organizationServiceFactory;

        // Cached values
        private string? _userId;
        private Guid? _organizationId;
        private Guid? _activeOrganizationId;
        private ApplicationUser? _currentUser;
        private bool _isInitialized = false;

        public UserContextService(
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager,
            IServiceProvider serviceProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
            // Use factory pattern to avoid circular dependency
            _organizationServiceFactory = async () => 
            {
                await Task.CompletedTask;
                return serviceProvider.GetRequiredService<OrganizationService>();
            };
        }

        /// <summary>
        /// Gets the current user's ID. Cached after first access.
        /// </summary>
        public async Task<string?> GetUserIdAsync()
        {
            await EnsureInitializedAsync();
            return _userId;
        }

        /// <summary>
        /// Gets the current user's OrganizationId. Cached after first access.
        /// DEPRECATED: Use GetActiveOrganizationIdAsync() for multi-org support
        /// </summary>
        public async Task<Guid?> GetOrganizationIdAsync()
        {
            await EnsureInitializedAsync();
            return _organizationId;
        }

        /// <summary>
        /// Gets the current user's active organization ID (new multi-org support).
        /// Returns null if user is not authenticated or has no active organization.
        /// Callers should handle null appropriately.
        /// </summary>
        public async Task<Guid?> GetActiveOrganizationIdAsync()
        {
            // Check if user is authenticated first
            if (!await IsAuthenticatedAsync())
            {
                return null; // Not authenticated - no organization
            }
            
            await EnsureInitializedAsync();
            
            // Return null if no active organization (e.g., fresh database, new user)
            if (!_activeOrganizationId.HasValue || _activeOrganizationId == Guid.Empty)
            {
                return null;
            }
            
            return _activeOrganizationId;
        }

        /// <summary>
        /// Gets the current ApplicationUser object. Cached after first access.
        /// </summary>
        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            await EnsureInitializedAsync();
            return _currentUser;
        }

        /// <summary>
        /// Checks if a user is authenticated.
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Gets the current user's email.
        /// </summary>
        public async Task<string?> GetUserEmailAsync()
        {
            await EnsureInitializedAsync();
            return _currentUser?.Email;
        }

        /// <summary>
        /// Gets the current user's full name.
        /// </summary>
        public async Task<string?> GetUserNameAsync()
        {
            await EnsureInitializedAsync();
            if (_currentUser != null)
            {
                return $"{_currentUser.FirstName} {_currentUser.LastName}".Trim();
            }
            return null;
        }

        /// <summary>
        /// Checks if the current user is in the specified role.
        /// </summary>
        public async Task<bool> IsInRoleAsync(string role)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return authState.User.IsInRole(role);
        }

        #region Multi-Organization Support

        /// <summary>
        /// Get all organizations the current user has access to
        /// </summary>
        public async Task<List<UserOrganization>> GetAccessibleOrganizationsAsync()
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return new List<UserOrganization>();

            var organizationService = await _organizationServiceFactory();
            return await organizationService.GetUserOrganizationsAsync(userId);
        }

        /// <summary>
        /// Get the current user's role in the active organization
        /// </summary>
        public async Task<string?> GetCurrentOrganizationRoleAsync()
        {
            var userId = await GetUserIdAsync();
            var activeOrganizationId = await GetActiveOrganizationIdAsync();

            if (string.IsNullOrEmpty(userId) || !activeOrganizationId.HasValue || activeOrganizationId == Guid.Empty)
                return null;

            var organizationService = await _organizationServiceFactory();
            return await organizationService.GetUserRoleForOrganizationAsync(userId, activeOrganizationId.Value);
        }

        /// <summary>
        /// Get the active organization entity
        /// </summary>
        public async Task<Organization?> GetActiveOrganizationAsync()
        {
            var activeOrganizationId = await GetActiveOrganizationIdAsync();
            if (!activeOrganizationId.HasValue || activeOrganizationId == Guid.Empty)
                return null;

            var organizationService = await _organizationServiceFactory();
            return await organizationService.GetOrganizationByIdAsync(activeOrganizationId.Value);
        }

        /// <summary>
        /// Get the organization entity by ID
        /// </summary>
        public async Task<Organization?> GetOrganizationByIdAsync(Guid organizationId)
        {
            if (organizationId == Guid.Empty)
                return null;

            var organizationService = await _organizationServiceFactory();
            return await organizationService.GetOrganizationByIdAsync(organizationId);
        }

        /// <summary>
        /// Switch the user's active organization
        /// </summary>
        public async Task<bool> SwitchOrganizationAsync(Guid organizationId)
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return false;

            // Verify user has access to this organization
            var organizationService = await _organizationServiceFactory();
            if (!await organizationService.CanAccessOrganizationAsync(userId, organizationId))
                return false;

            // Update user's active organization
            var user = await GetCurrentUserAsync();
            if (user == null)
                return false;

            user.ActiveOrganizationId = organizationId;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Refresh cache
                await RefreshAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the current user has a specific permission in their active organization
        /// </summary>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            var role = await GetCurrentOrganizationRoleAsync();
            if (string.IsNullOrEmpty(role))
                return false;

            // Permission checks based on role
            return permission.ToLower() switch
            {
                "organizations.create" => role == ApplicationConstants.OrganizationRoles.Owner,
                "organizations.delete" => role == ApplicationConstants.OrganizationRoles.Owner,
                "organizations.backup" => role == ApplicationConstants.OrganizationRoles.Owner,
                "organizations.deletedata" => role == ApplicationConstants.OrganizationRoles.Owner,
                "settings.edit" => ApplicationConstants.OrganizationRoles.CanEditSettings(role),
                "settings.retention" => role == ApplicationConstants.OrganizationRoles.Owner || role == ApplicationConstants.OrganizationRoles.Administrator,
                "users.manage" => ApplicationConstants.OrganizationRoles.CanManageUsers(role),
                "properties.manage" => role != ApplicationConstants.OrganizationRoles.User,
                _ => false
            };
        }

        /// <summary>
        /// Check if the current user is an account owner (owns at least one organization)
        /// </summary>
        public async Task<bool> IsAccountOwnerAsync()
        {
            var userId = await GetUserIdAsync();
            if (string.IsNullOrEmpty(userId))
                return false;

            var organizationService = await _organizationServiceFactory();
            var ownedOrgs = await organizationService.GetOwnedOrganizationsAsync(userId);
            return ownedOrgs.Any();
        }

        #endregion

        /// <summary>
        /// Forces a refresh of the cached user data.
        /// Call this if user data has been updated and you need to reload it.
        /// </summary>
        public async Task RefreshAsync()
        {
            _isInitialized = false;
            _userId = null;
            _organizationId = null;
            _activeOrganizationId = null;
            _currentUser = null;
            await EnsureInitializedAsync();
        }

        /// <summary>
        /// Initializes the user context by loading user data from the database.
        /// This is called automatically on first access and cached for subsequent calls.
        /// </summary>
        private async Task EnsureInitializedAsync()
        {
            if (_isInitialized)
                return;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var claimsUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(claimsUserId))
                {
                    _userId = claimsUserId;
                }
                {
                    _currentUser = await _userManager.FindByIdAsync(_userId!);
                    if (_currentUser != null)
                    {
                        _activeOrganizationId = _currentUser.ActiveOrganizationId; // New multi-org
                    }
                }
            }

            _isInitialized = true;
        }
    }
}
