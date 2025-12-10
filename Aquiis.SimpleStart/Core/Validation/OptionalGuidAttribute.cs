using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Core.Validation;

/// <summary>
/// Validates that an optional Guid property, if provided, is not Guid.Empty.
/// Use this for Guid? properties where null is acceptable but Guid.Empty is not.
/// 
/// Example: LeaseId on MaintenanceRequest - can be null (no lease yet) but shouldn't be Guid.Empty (invalid reference)
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class OptionalGuidAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of OptionalGuidAttribute with a default error message.
    /// </summary>
    public OptionalGuidAttribute()
        : base("The {0} field cannot be empty if provided. Either leave it null or provide a valid value.")
    {
    }

    /// <summary>
    /// Initializes a new instance of OptionalGuidAttribute with a custom error message.
    /// </summary>
    /// <param name="errorMessage">The error message to display when validation fails.</param>
    public OptionalGuidAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    /// <summary>
    /// Validates that if the value is not null, it must not be Guid.Empty.
    /// </summary>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Null is acceptable for optional fields
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // Type check
        if (value is not Guid guidValue)
        {
            return new ValidationResult(
                $"The {validationContext.DisplayName} field must be a valid Guid or null.",
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        // Reject Guid.Empty (if you provide a value, it must be real)
        if (guidValue == Guid.Empty)
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        return ValidationResult.Success;
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        if (value is not Guid guidValue)
            return false;

        return guidValue != Guid.Empty;
    }
}
