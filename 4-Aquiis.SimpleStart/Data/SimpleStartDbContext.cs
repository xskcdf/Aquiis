using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Entities;

namespace Aquiis.SimpleStart.Data;

/// <summary>
/// SimpleStart database context for Identity management.
/// Handles all ASP.NET Core Identity tables and SimpleStart-specific user data.
/// Shares the same database as ApplicationDbContext using the same connection string.
/// </summary>
public class SimpleStartDbContext : IdentityDbContext<ApplicationUser>
{
    public SimpleStartDbContext(DbContextOptions<SimpleStartDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Identity table configuration is handled by base IdentityDbContext
        // Add any SimpleStart-specific user configurations here if needed
    }
}
