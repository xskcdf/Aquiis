using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Core.Validation;

/// <summary>
/// Validates that a Guid property has a value other than Guid.Empty.
/// Use this instead of [Required] for non-nullable Guid properties.
/// 
/// Note: For nullable Guid? properties, use [Required] to check for null,
/// and optionally combine with [RequiredGuid] to also reject Guid.Empty.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredGuidAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of RequiredGuidAttribute with a default error message.
    /// </summary>
    public RequiredGuidAttribute()
        : base("The {0} field is required and cannot be empty.")
    {
    }

    /// <summary>
    /// Initializes a new instance of RequiredGuidAttribute with a custom error message.
    /// </summary>
    /// <param name="errorMessage">The error message to display when validation fails.</param>
    public RequiredGuidAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    /// <summary>
    /// Validates that the value is not null, not Guid.Empty, and is a valid Guid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>ValidationResult.Success if valid, otherwise a ValidationResult with error message.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Null check (for Guid? properties)
        if (value == null)
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        // Type check
        if (value is not Guid guidValue)
        {
            return new ValidationResult(
                $"The {validationContext.DisplayName} field must be a valid Guid.",
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        // Empty Guid check
        if (guidValue == Guid.Empty)
        {
            return new ValidationResult(
                FormatErrorMessage(validationContext.DisplayName),
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        return ValidationResult.Success;
    }

    /// <summary>
    /// Simple validation for attribute usage without ValidationContext.
    /// </summary>
    public override bool IsValid(object? value)
    {
        if (value == null)
            return false;

        if (value is not Guid guidValue)
            return false;

        return guidValue != Guid.Empty;
    }
}
