using Aquiis.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Aquiis.Professional.Services;

/// <summary>
/// Path service for web/server applications.
/// Uses standard server file system paths.
/// </summary>
public class WebPathService : IPathService
{
    private readonly IConfiguration _configuration;
    
    public WebPathService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public bool IsActive => true;
    
    public async Task<string> GetConnectionStringAsync(object configuration)
    {
        if (configuration is IConfiguration config)
        {
            return await Task.Run(() => config.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
        }
        throw new ArgumentException("Configuration must be IConfiguration", nameof(configuration));
    }
    
    public async Task<string> GetDatabasePathAsync()
    {
        var connectionString = await GetConnectionStringAsync(_configuration);
        // Extract Data Source from connection string
        var dataSource = connectionString.Split(';')
            .FirstOrDefault(s => s.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
        
        if (dataSource != null)
        {
            return dataSource.Split('=')[1].Trim();
        }
        
        return "aquiis.db"; // Default
    }
    
    public async Task<string> GetUserDataPathAsync()
    {
        return await Task.Run(() => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    }
}
