namespace Aquiis.Core.Interfaces;

/// <summary>
/// Platform-agnostic interface for the application's database context.
/// This allows the Application layer to reference the DbContext without knowing Infrastructure details.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
