namespace Aquiis.Application.Models;

/// <summary>
/// View model for SMS configuration in the UI.
/// Combines settings from OrganizationSMSSettings for display and editing.
/// </summary>
public class SMSConfigurationModel
{
    /// <summary>
    /// Twilio Account SID (unencrypted for display/editing)
    /// </summary>
    public string AccountSid { get; set; } = string.Empty;

    /// <summary>
    /// Twilio Auth Token (unencrypted for display/editing)
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// Phone number to send from
    /// </summary>
    public string FromPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Whether SMS is currently enabled
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Whether credentials have been verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// SMS messages sent today
    /// </summary>
    public int SMSSentToday { get; set; }

    /// <summary>
    /// SMS messages sent this month
    /// </summary>
    public int SMSSentThisMonth { get; set; }

    /// <summary>
    /// Last time SMS was sent
    /// </summary>
    public DateTime? LastSMSSentOn { get; set; }

    /// <summary>
    /// Last time credentials were verified
    /// </summary>
    public DateTime? LastVerifiedOn { get; set; }

    /// <summary>
    /// Account balance (if available)
    /// </summary>
    public decimal? AccountBalance { get; set; }

    /// <summary>
    /// Cost per SMS (if available)
    /// </summary>
    public decimal? CostPerSMS { get; set; }

    /// <summary>
    /// Any error message from last operation
    /// </summary>
    public string? LastError { get; set; }
}
