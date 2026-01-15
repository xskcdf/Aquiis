namespace Aquiis.Core.Utilities;

/// <summary>
/// Provides string sanitization and normalization utilities for consistent data storage.
/// </summary>
public static class StringSanitizer
{
    /// <summary>
    /// Trims leading and trailing whitespace from a string.
    /// Returns empty string if input is null or whitespace.
    /// </summary>
    public static string Trim(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Trims and converts to lowercase for case-insensitive comparisons.
    /// Useful for emails, usernames, etc.
    /// </summary>
    public static string NormalizeForComparison(string? value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    /// <summary>
    /// Normalizes email addresses: trims and converts to lowercase.
    /// </summary>
    public static string NormalizeEmail(string? email)
    {
        return email?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    /// <summary>
    /// Normalizes phone numbers: removes all non-digit characters.
    /// Example: "(555) 123-4567" becomes "5551234567"
    /// </summary>
    public static string NormalizePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Formats a normalized phone number for display.
    /// Example: "5551234567" becomes "(555) 123-4567"
    /// </summary>
    public static string FormatPhoneNumber(string? phoneNumber)
    {
        var normalized = NormalizePhoneNumber(phoneNumber);
        
        if (normalized.Length == 10)
        {
            return $"({normalized.Substring(0, 3)}) {normalized.Substring(3, 3)}-{normalized.Substring(6, 4)}";
        }
        else if (normalized.Length == 11 && normalized.StartsWith("1"))
        {
            return $"+1 ({normalized.Substring(1, 3)}) {normalized.Substring(4, 3)}-{normalized.Substring(7, 4)}";
        }

        return phoneNumber ?? string.Empty;
    }

    /// <summary>
    /// Collapses multiple consecutive spaces into a single space and trims.
    /// Example: "Hello    World  " becomes "Hello World"
    /// </summary>
    public static string CollapseWhitespace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return string.Join(" ", value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    /// <summary>
    /// Removes all whitespace from a string.
    /// Useful for IDs, codes, etc.
    /// </summary>
    public static string RemoveWhitespace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }

    /// <summary>
    /// Sanitizes a string for safe display by trimming and collapsing whitespace.
    /// </summary>
    public static string Sanitize(string? value)
    {
        return CollapseWhitespace(value);
    }
}
