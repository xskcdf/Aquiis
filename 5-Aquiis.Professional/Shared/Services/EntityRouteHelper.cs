using Aquiis.Core.Entities;

namespace Aquiis.Professional.Shared.Services;

/// <summary>
/// Provides centralized mapping between entity types and their navigation routes.
/// This ensures consistent URL generation across the application when navigating to entity details.
/// RESTful pattern: /resource/{id} for view, /resource/{id}/edit for edit, /resource for list
/// </summary>
public static class EntityRouteHelper
{
    private static readonly Dictionary<string, string> RouteMap = new()
    {
        { "Lease", "/propertymanagement/leases" },
        { "Payment", "/propertymanagement/payments" },
        { "Invoice", "/propertymanagement/invoices" },
        { "Maintenance", "/propertymanagement/maintenance" },
        { "Application", "/propertymanagement/applications" },
        { "Property", "/propertymanagement/properties" },
        { "Tenant", "/propertymanagement/tenants" },
        { "Prospect", "/PropertyManagement/ProspectiveTenants" },
        { "Inspection", "/propertymanagement/inspections" },
        { "LeaseOffer", "/propertymanagement/leaseoffers" },
        { "Checklist", "/propertymanagement/checklists" },
        { "Organization", "/administration/organizations" }
    };

    /// <summary>
    /// Gets the full navigation route for viewing an entity (RESTful: /resource/{id})
    /// </summary>
    /// <param name="entityType">The type of entity (e.g., "Lease", "Payment", "Maintenance")</param>
    /// <param name="entityId">The unique identifier of the entity</param>
    /// <returns>The full route path including the entity ID, or "/" if the entity type is not mapped</returns>
    public static string GetEntityRoute(string? entityType, Guid entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return "/";
        }

        if (RouteMap.TryGetValue(entityType, out var route))
        {
            return $"{route}/{entityId}";
        }
        
        // Fallback to home if entity type not found
        return "/"; 
    }

    /// <summary>
    /// Gets the route for an entity action (RESTful: /resource/{id}/action)
    /// </summary>
    /// <param name="entityType">The type of entity</param>
    /// <param name="entityId">The unique identifier of the entity</param>
    /// <param name="action">The action (e.g., "edit", "delete", "approve")</param>
    /// <returns>The full route path, or "/" if not mapped</returns>
    public static string GetEntityActionRoute(string? entityType, Guid entityId, string action)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return "/";
        }

        if (RouteMap.TryGetValue(entityType, out var route))
        {
            return $"{route}/{entityId}/{action}";
        }
        
        return "/";
    }

    /// <summary>
    /// Gets the list route for an entity type (RESTful: /resource)
    /// </summary>
    /// <param name="entityType">The type of entity</param>
    /// <returns>The list route path, or "/" if not mapped</returns>
    public static string GetListRoute(string? entityType)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return "/";
        }

        if (RouteMap.TryGetValue(entityType, out var route))
        {
            return route;
        }
        
        return "/";
    }

    /// <summary>
    /// Gets the create route for an entity type (RESTful: /resource/create)
    /// </summary>
    /// <param name="entityType">The type of entity</param>
    /// <returns>The create route path, or "/" if not mapped</returns>
    public static string GetCreateRoute(string? entityType)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return "/";
        }

        if (RouteMap.TryGetValue(entityType, out var route))
        {
            return $"{route}/create";
        }
        
        return "/";
    }

    /// <summary>
    /// Checks if a route mapping exists for the given entity type.
    /// </summary>
    /// <param name="entityType">The type of entity to check</param>
    /// <returns>True if a route mapping exists, false otherwise</returns>
    public static bool HasRoute(string? entityType)
    {
        return !string.IsNullOrWhiteSpace(entityType) && RouteMap.ContainsKey(entityType);
    }

    /// <summary>
    /// Gets all supported entity types that have route mappings.
    /// </summary>
    /// <returns>A collection of supported entity type names</returns>
    public static IEnumerable<string> GetSupportedEntityTypes()
    {
        return RouteMap.Keys;
    }
}