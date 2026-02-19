using System;

namespace Aquiis.Core.Entities;

/// <summary>
/// User profile information stored in business context (ApplicationDbContext).
/// Separate from Identity (AspNetUsers) to enable single-context queries with business entities.
/// Email is denormalized for query simplification (considered read-only in B2B context).
/// </summary>
public class UserProfile : BaseModel
{
    /// <summary>
    /// Foreign key to AspNetUsers.Id (Identity context).
    /// This links the profile to the authentication user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    // Personal Information (denormalized from Identity for query efficiency)
    
    /// <summary>
    /// User's email address (cached from AspNetUsers).
    /// Considered read-only - changes require sync with Identity table.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    // Organization Context
    
    /// <summary>
    /// User's "home" organization - their primary/default organization.
    /// Shadows BaseModel.OrganizationId to keep it nullable (users can exist before org assignment).
    /// </summary>
    public new Guid? OrganizationId { get; set; }
    
    /// <summary>
    /// Currently active organization the user is viewing/working with.
    /// Can differ from home organization for users with multi-org access.
    /// </summary>
    public Guid? ActiveOrganizationId { get; set; }
    
    // Computed Properties
    
    /// <summary>
    /// Full name combining first and last name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Display name for UI - uses full name if available, falls back to email.
    /// </summary>
    public string DisplayName => string.IsNullOrWhiteSpace(FullName) ? Email : FullName;
}
