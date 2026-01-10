using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aquiis.Professional.Entities;

namespace Aquiis.Professional.Data;

/// <summary>
/// Professional database context for Identity management.
/// Handles all ASP.NET Core Identity tables and Professional-specific user data.
/// Shares the same database as ApplicationDbContext using the same connection string.
/// </summary>
public class ProfessionalDbContext : IdentityDbContext<ApplicationUser>
{
    public ProfessionalDbContext(DbContextOptions<ProfessionalDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Identity table configuration is handled by base IdentityDbContext
        // Add any Professional-specific user configurations here if needed
    }
}
