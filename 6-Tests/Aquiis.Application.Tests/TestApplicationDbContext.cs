using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.Infrastructure.Data;

namespace Aquiis.Application.Tests;

/// <summary>
/// Minimal ApplicationUser entity for testing purposes only.
/// This is a simplified version used only in unit tests.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public Guid ActiveOrganizationId { get; set; } = Guid.Empty;
}

/// <summary>
/// Test-specific DbContext that extends ApplicationDbContext with Identity support.
/// This allows tests to work with ApplicationUser entities in an in-memory database.
/// </summary>
public class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions options)
        : base((DbContextOptions<ApplicationDbContext>)options)
    {
    }

    // Expose ApplicationUser DbSet for test scenarios
    public DbSet<ApplicationUser> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser entity for tests
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.Email);
        });
    }
}
