using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Aquiis.SimpleStart.Shared.Services;

public static class ElectronPathService
{
    /// <summary>
    /// Gets the database file path. Uses Electron's user data directory when running as desktop app,
    /// otherwise uses the local Data folder for web mode.
    /// </summary>
    public static async Task<string> GetDatabasePathAsync()
    {
        if (HybridSupport.IsElectronActive)
        {
            var userDataPath = await Electron.App.GetPathAsync(PathName.UserData);
            var dbPath = Path.Combine(userDataPath, "app.db");
            
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
            // Web mode - use local Data folder
            return "Data/app.db";
        }
    }

    /// <summary>
    /// Gets the connection string for the database.
    /// </summary>
    public static async Task<string> GetConnectionStringAsync()
    {
        var dbPath = await GetDatabasePathAsync();
        return $"DataSource={dbPath};Cache=Shared";
    }
}
