namespace Aquiis.Infrastructure.Services;

/// <summary>
/// Singleton service tracking database encryption unlock state during app lifecycle
/// </summary>
public class DatabaseUnlockState
{
    public bool NeedsUnlock { get; set; }
    public string? DatabasePath { get; set; }
    public string? ConnectionString { get; set; }
    
    // Event to notify when unlock succeeds
    public event Action? OnUnlockSuccess;
    
    public void NotifyUnlockSuccess() => OnUnlockSuccess?.Invoke();
}
