using Microsoft.AspNetCore.Identity;

namespace Aquiis.SimpleStart.Shared.Components.Account;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string OrganizationId { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime? LastLoginDate { get; set; }
    public DateTime? PreviousLoginDate { get; set; }
    public int LoginCount { get; set; } = 0;
    public string? LastLoginIP { get; set; }
}

