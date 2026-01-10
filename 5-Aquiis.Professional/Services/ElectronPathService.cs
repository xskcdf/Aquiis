using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Extensions.Configuration;
using Aquiis.Core.Interfaces;

namespace Aquiis.Professional.Services;

/// <summary>
/// Electron-specific implementation of path service.
/// Manages file paths and connection strings for Electron desktop applications.
/// </summary>
public class ElectronPathService : IPathService
{
    private readonly IConfiguration _configuration;

    public ElectronPathService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public bool IsActive => HybridSupport.IsElectronActive;

    /// <inheritdoc/>
    public async Task<string> GetConnectionStringAsync(object configuration)
    {
        var dbPath = await GetDatabasePathAsync();
        return $"DataSource={dbPath};Cache=Shared";
    }

    /// <inheritdoc/>
    public async Task<string> GetDatabasePathAsync()
    {
        var dbFileName = _configuration["ApplicationSettings:DatabaseFileName"] ?? "app.db";
        
        if (HybridSupport.IsElectronActive)
        {
            var userDataPath = await GetUserDataPathAsync();
            var dbPath = Path.Combine(userDataPath, dbFileName);
            
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            return dbPath;
        }
        else
        {
            // Fallback to local path if not in Electron mode
            var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            return Path.Combine(dataDir, dbFileName);
        }
    }

    /// <inheritdoc/>
    public async Task<string> GetUserDataPathAsync()
    {
        if (HybridSupport.IsElectronActive)
        {
            return await Electron.App.GetPathAsync(PathName.UserData);
        }
        else
        {
            // Fallback for non-Electron mode
            return Path.Combine(Directory.GetCurrentDirectory(), "Data");
        }
    }

}
