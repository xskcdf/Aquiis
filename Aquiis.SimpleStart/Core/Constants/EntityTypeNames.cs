using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Constants;

/// <summary>
/// Centralized entity type names for integration tables (Notes, Audit Logs, etc.)
/// Uses fully-qualified type names to prevent collisions with external systems
/// </summary>
public static class EntityTypeNames
{
    // Property Management Domain
    public const string Property = "Aquiis.SimpleStart.Core.Entities.Property";
    public const string Tenant = "Aquiis.SimpleStart.Core.Entities.Tenant";
    public const string Lease = "Aquiis.SimpleStart.Core.Entities.Lease";
    public const string LeaseOffer = "Aquiis.SimpleStart.Core.Entities.LeaseOffer";
    public const string Invoice = "Aquiis.SimpleStart.Core.Entities.Invoice";
    public const string Payment = "Aquiis.SimpleStart.Core.Entities.Payment";
    public const string MaintenanceRequest = "Aquiis.SimpleStart.Core.Entities.MaintenanceRequest";
    public const string Inspection = "Aquiis.SimpleStart.Core.Entities.Inspection";
    public const string Document = "Aquiis.SimpleStart.Core.Entities.Document";
    
    // Application/Prospect Domain
    public const string ProspectiveTenant = "Aquiis.SimpleStart.Core.Entities.ProspectiveTenant";
    public const string Application = "Aquiis.SimpleStart.Core.Entities.Application";
    public const string Tour = "Aquiis.SimpleStart.Core.Entities.Tour";
    
    // Checklist Domain
    public const string Checklist = "Aquiis.SimpleStart.Core.Entities.Checklist";
    public const string ChecklistTemplate = "Aquiis.SimpleStart.Core.Entities.ChecklistTemplate";
    
    // Calendar/Events
    public const string CalendarEvent = "Aquiis.SimpleStart.Core.Entities.CalendarEvent";
    
    // Security Deposits
    public const string SecurityDepositPool = "Aquiis.SimpleStart.Core.Entities.SecurityDepositPool";
    public const string SecurityDepositTransaction = "Aquiis.SimpleStart.Core.Entities.SecurityDepositTransaction";
    
    /// <summary>
    /// Get the fully-qualified type name for an entity type
    /// </summary>
    public static string GetTypeName<T>() where T : BaseModel
    {
        return typeof(T).FullName ?? typeof(T).Name;
    }
    
    /// <summary>
    /// Get the display name (simple name) from a fully-qualified type name
    /// </summary>
    public static string GetDisplayName(string fullyQualifiedName)
    {
        return fullyQualifiedName.Split('.').Last();
    }
    
    /// <summary>
    /// Validate that an entity type string is recognized
    /// </summary>
    public static bool IsValidEntityType(string entityType)
    {
        return typeof(EntityTypeNames)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Contains(entityType);
    }
}
