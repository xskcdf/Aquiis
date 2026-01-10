namespace Aquiis.Application.Models;

/// <summary>
/// View model for email configuration in the UI.
/// Combines settings from OrganizationEmailSettings for display and editing.
/// </summary>
public class EmailConfigurationModel
{
    /// <summary>
    /// SendGrid API Key (unencrypted for display/editing)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Email address to send from
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Display name for sender
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// Whether email is currently enabled
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Whether credentials have been verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Daily email limit
    /// </summary>
    public int DailyLimit { get; set; }

    /// <summary>
    /// Monthly email limit
    /// </summary>
    public int MonthlyLimit { get; set; }

    /// <summary>
    /// Emails sent today
    /// </summary>
    public int EmailsSentToday { get; set; }

    /// <summary>
    /// Emails sent this month
    /// </summary>
    public int EmailsSentThisMonth { get; set; }

    /// <summary>
    /// Last time email was sent
    /// </summary>
    public DateTime? LastEmailSentOn { get; set; }

    /// <summary>
    /// Last time credentials were verified
    /// </summary>
    public DateTime? LastVerifiedOn { get; set; }

    /// <summary>
    /// Any error message from last operation
    /// </summary>
    public string? LastError { get; set; }
}
