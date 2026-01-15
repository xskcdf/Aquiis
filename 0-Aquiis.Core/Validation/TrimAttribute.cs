using System.ComponentModel.DataAnnotations;

namespace Aquiis.Core.Validation;

/// <summary>
/// Validation attribute that automatically trims string values during model binding.
/// Removes leading and trailing whitespace from the property value.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class TrimAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {
            var trimmedValue = stringValue.Trim();
            
            // Use reflection to set the trimmed value back to the property
            var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(validationContext.ObjectInstance, trimmedValue);
            }
        }

        return ValidationResult.Success;
    }
}
