using Aquiis.Core.Entities;

namespace Aquiis.Application.Constants;

/// <summary>
/// Centralized entity type names for integration tables (Notes, Audit Logs, etc.)
/// Uses fully-qualified type names to prevent collisions with external systems
/// </summary>
public static class EntityTypeNames
{
    // Property Management Domain
    public const string Property = "Aquiis.Core.Entities.Property";
    public const string Tenant = "Aquiis.Core.Entities.Tenant";
    public const string Lease = "Aquiis.Core.Entities.Lease";
    public const string LeaseOffer = "Aquiis.Core.Entities.LeaseOffer";
    public const string Invoice = "Aquiis.Core.Entities.Invoice";
    public const string Payment = "Aquiis.Core.Entities.Payment";
    public const string MaintenanceRequest = "Aquiis.Core.Entities.MaintenanceRequest";
    public const string Inspection = "Aquiis.Core.Entities.Inspection";
    public const string Document = "Aquiis.Core.Entities.Document";
    
    // Application/Prospect Domain
    public const string ProspectiveTenant = "Aquiis.Core.Entities.ProspectiveTenant";
    public const string Application = "Aquiis.Core.Entities.Application";
    public const string Tour = "Aquiis.Core.Entities.Tour";
    
    // Checklist Domain
    public const string Checklist = "Aquiis.Core.Entities.Checklist";
    public const string ChecklistTemplate = "Aquiis.Core.Entities.ChecklistTemplate";
    
    // Calendar/Events
    public const string CalendarEvent = "Aquiis.Core.Entities.CalendarEvent";
    
    // Security Deposits
    public const string SecurityDepositPool = "Aquiis.Core.Entities.SecurityDepositPool";
    public const string SecurityDepositTransaction = "Aquiis.Core.Entities.SecurityDepositTransaction";
    
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
