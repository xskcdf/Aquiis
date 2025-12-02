using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Aquiis.SimpleStart.Components.Account;
using System.Security.Claims;

namespace Aquiis.SimpleStart.Shared.Services
{

    /// <summary>
    /// Provides cached access to the current user's context information including OrganizationId.
    /// This service is scoped per Blazor circuit, so the data is cached for the user's session.
    /// </summary>
    public class UserContextService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        // Cached values
        private string? _userId;
        private string? _organizationId;
        private ApplicationUser? _currentUser;
        private bool _isInitialized = false;

        public UserContextService(
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<ApplicationUser> userManager)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
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
        /// </summary>
        public async Task<string?> GetOrganizationIdAsync()
        {
            await EnsureInitializedAsync();
            return _organizationId;
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

        /// <summary>
        /// Forces a refresh of the cached user data.
        /// Call this if user data has been updated and you need to reload it.
        /// </summary>
        public async Task RefreshAsync()
        {
            _isInitialized = false;
            _userId = null;
            _organizationId = null;
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
                _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(_userId))
                {
                    _currentUser = await _userManager.FindByIdAsync(_userId);
                    if (_currentUser != null)
                    {
                        _organizationId = _currentUser.OrganizationId;
                    }
                }
            }

            _isInitialized = true;
        }
    }
}
