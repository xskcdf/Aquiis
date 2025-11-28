using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Models;
using Aquiis.SimpleStart.Components.Administration.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Services
{
    public class SchemaValidationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationSettings _settings;
        private readonly ILogger<SchemaValidationService> _logger;

        public SchemaValidationService(
            ApplicationDbContext dbContext,
            IOptions<ApplicationSettings> settings,
            ILogger<SchemaValidationService> logger)
        {
            _dbContext = dbContext;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Validates that the database schema version matches the application's expected version
        /// </summary>
        public async Task<(bool IsValid, string Message, string? DatabaseVersion)> ValidateSchemaVersionAsync()
        {
            try
            {
                // Get the current schema version from database
                var currentVersion = await _dbContext.SchemaVersions
                    .OrderByDescending(v => v.AppliedOn)
                    .FirstOrDefaultAsync();

                if (currentVersion == null)
                {
                    _logger.LogWarning("No schema version records found in database");
                    return (false, "No schema version found. Database may be corrupted or incomplete.", null);
                }

                var expectedVersion = _settings.SchemaVersion;
                var dbVersion = currentVersion.Version;

                if (dbVersion != expectedVersion)
                {
                    _logger.LogWarning("Schema version mismatch. Expected: {Expected}, Database: {Actual}", 
                        expectedVersion, dbVersion);
                    return (false, 
                        $"Schema version mismatch! Application expects v{expectedVersion} but database is v{dbVersion}. Please update the application or restore a compatible backup.",
                        dbVersion);
                }

                _logger.LogInformation("Schema version validated successfully: {Version}", dbVersion);
                return (true, $"Schema version {dbVersion} is valid", dbVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating schema version");
                return (false, $"Error validating schema: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Updates or creates the schema version record
        /// </summary>
        public async Task UpdateSchemaVersionAsync(string version, string description = "")
        {
            try
            {
                _logger.LogInformation("Creating schema version record: Version={Version}, Description={Description}", version, description);
                
                var schemaVersion = new SchemaVersion
                {
                    Version = version,
                    AppliedOn = DateTime.UtcNow,
                    Description = description
                };

                _dbContext.SchemaVersions.Add(schemaVersion);
                _logger.LogInformation("Schema version entity added to context, saving changes...");
                
                var saved = await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SaveChanges completed. Rows affected: {Count}", saved);

                _logger.LogInformation("Schema version updated to {Version}", version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update schema version");
                throw;
            }
        }

        /// <summary>
        /// Gets the current database schema version
        /// </summary>
        public async Task<string?> GetCurrentSchemaVersionAsync()
        {
            try
            {
                // Check if table exists first
                var tableExists = await _dbContext.Database.ExecuteSqlRawAsync(
                    "SELECT 1 FROM sqlite_master WHERE type='table' AND name='SchemaVersions'") >= 0;
                
                if (!tableExists)
                {
                    _logger.LogWarning("SchemaVersions table does not exist");
                    return null;
                }

                var currentVersion = await _dbContext.SchemaVersions
                    .OrderByDescending(v => v.AppliedOn)
                    .FirstOrDefaultAsync();

                if (currentVersion == null)
                {
                    _logger.LogInformation("SchemaVersions table exists but has no records");
                }

                return currentVersion?.Version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current schema version");
                return null;
            }
        }
    }
}
