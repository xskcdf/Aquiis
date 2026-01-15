using Aquiis.Core.Entities;

namespace Aquiis.UI.Shared.Components.Entities.OrganizationUsers;


/// <summary>
/// View model representing a user's membership in an organization.
/// Combines data from OrganizationUser entity and ApplicationUser (AspNetUsers).
/// </summary>
public class OrganizationUserViewModel
{
    // From OrganizationUser entity
    public Guid Id { get; set; } // OrganizationUser.Id
    public Guid OrganizationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime GrantedOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? GrantedBy { get; set; } // UserId of granter
    public string? GrantedByEmail { get; set; } // Email of granter (from UserProfile join)
    
    // From UserProfile
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayRole => Role;
}