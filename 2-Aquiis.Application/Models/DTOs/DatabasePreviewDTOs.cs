namespace Aquiis.Application.Models.DTOs;

/// <summary>
/// DTO for database preview summary data
/// </summary>
public class DatabasePreviewData
{
    public int PropertyCount { get; set; }
    public int TenantCount { get; set; }
    public int LeaseCount { get; set; }
    public int InvoiceCount { get; set; }
    public int PaymentCount { get; set; }
    
    public List<PropertyPreview> Properties { get; set; } = new();
    public List<TenantPreview> Tenants { get; set; } = new();
    public List<LeasePreview> Leases { get; set; } = new();
}

/// <summary>
/// DTO for property preview in read-only database view
/// </summary>
public class PropertyPreview
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Units { get; set; }
    public decimal? MonthlyRent { get; set; }
}

/// <summary>
/// DTO for tenant preview in read-only database view
/// </summary>
public class TenantPreview
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// DTO for lease preview in read-only database view
/// </summary>
public class LeasePreview
{
    public Guid Id { get; set; }
    public string PropertyAddress { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Result object for database operations
/// </summary>
public class DatabaseOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public static DatabaseOperationResult SuccessResult(string message = "Operation successful")
        => new() { Success = true, Message = message };
    
    public static DatabaseOperationResult FailureResult(string message)
        => new() { Success = false, Message = message };
}
