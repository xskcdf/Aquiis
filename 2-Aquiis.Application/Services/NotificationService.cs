
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aquiis.Application.Services;
public class NotificationService : BaseService<Notification>
{
    private readonly IEmailService _emailService;
    private readonly ISMSService _smsService;
    private new readonly ILogger<NotificationService> _logger;

    public NotificationService(
        ApplicationDbContext context,
        IUserContextService userContext,
        IEmailService emailService,
        ISMSService smsService,
        IOptions<ApplicationSettings> appSettings,
        ILogger<NotificationService> logger)
        : base(context, logger, userContext, appSettings)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Create and send a notification to a user
    /// </summary>
    public async Task<Notification> SendNotificationAsync(
        string recipientUserId,
        string title,
        string message,
        string type,
        string category,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null)
    {
        var organizationId = await _userContext.GetActiveOrganizationIdAsync();

        // Get user preferences
        var preferences = await GetNotificationPreferencesAsync(recipientUserId);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId!.Value,
            RecipientUserId = recipientUserId,
            Title = title,
            Message = message,
            Type = type,
            Category = category,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            SentOn = DateTime.UtcNow,
            IsRead = false,
            SendInApp = preferences.EnableInAppNotifications,
            SendEmail = preferences.EnableEmailNotifications && ShouldSendEmail(category, preferences),
            SendSMS = preferences.EnableSMSNotifications && ShouldSendSMS(category, preferences)
        };

        // Save in-app notification
        await CreateAsync(notification);

        // Send email if enabled
        if (notification.SendEmail && !string.IsNullOrEmpty(preferences.EmailAddress))
        {
            try
            {
                await _emailService.SendEmailAsync(
                    preferences.EmailAddress,
                    title,
                    message);

                notification.EmailSent = true;
                notification.EmailSentOn = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email notification to {recipientUserId}");
                notification.EmailError = ex.Message;
            }
        }

        // Send SMS if enabled
        if (notification.SendSMS && !string.IsNullOrEmpty(preferences.PhoneNumber))
        {
            try
            {
                await _smsService.SendSMSAsync(
                    preferences.PhoneNumber,
                    $"{title}: {message}");

                notification.SMSSent = true;
                notification.SMSSentOn = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS notification to {recipientUserId}");
                notification.SMSError = ex.Message;
            }
        }

        await UpdateAsync(notification);

        return notification;
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await GetByIdAsync(notificationId);
        if (notification == null) return;

        notification.IsRead = true;
        notification.ReadOn = DateTime.UtcNow;

        await UpdateAsync(notification);
    }

    /// <summary>
    /// Mark all notifications as read for the current user
    /// </summary>
    public async Task MarkAllAsReadAsync(List<Notification> notifications)
    {
        foreach (var notification in notifications)
        {
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadOn = DateTime.UtcNow;
                await UpdateAsync(notification);
            }
        }
    }

    /// <summary>
    /// Get unread notifications for current user
    /// Returns empty list if user is not authenticated or has no organization.
    /// </summary>
    public async Task<List<Notification>> GetUnreadNotificationsAsync()
    {
        var userId = await _userContext.GetUserIdAsync();
        var organizationId = await _userContext.GetActiveOrganizationIdAsync();

        // Return empty list if not authenticated or no organization
        if (string.IsNullOrEmpty(userId) || !organizationId.HasValue)
        {
            return new List<Notification>();
        }

        return await _context.Notifications
            .Where(n => n.OrganizationId == organizationId
                && n.RecipientUserId == userId
                && !n.IsRead
                && !n.IsDeleted)
            .OrderByDescending(n => n.SentOn)
            .Take(50)
            .ToListAsync();
    }

    /// <summary>
    /// Get notification history for current user
    /// </summary>
    public async Task<List<Notification>> GetNotificationHistoryAsync(int count = 100)
    {
        var userId = await _userContext.GetUserIdAsync();
        var organizationId = await _userContext.GetActiveOrganizationIdAsync();

        return await _context.Notifications
            .Where(n => n.OrganizationId == organizationId
                && n.RecipientUserId == userId
                && !n.IsDeleted)
            .OrderByDescending(n => n.SentOn)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Get notification preferences for current user
    /// </summary>
    public async Task<NotificationPreferences> GetUserPreferencesAsync()
    {
        var userId = await _userContext.GetUserIdAsync();
        return await GetNotificationPreferencesAsync(userId!);
    }

    /// <summary>
    /// Update notification preferences for current user
    /// </summary>
    public async Task<NotificationPreferences> UpdateUserPreferencesAsync(NotificationPreferences preferences)
    {
        var userId = await _userContext.GetUserIdAsync();
        var organizationId = await _userContext.GetActiveOrganizationIdAsync();

        // Ensure the preferences belong to the current user and organization
        if (preferences.UserId != userId || preferences.OrganizationId != organizationId)
        {
            throw new UnauthorizedAccessException("Cannot update preferences for another user");
        }

        _context.NotificationPreferences.Update(preferences);
        await _context.SaveChangesAsync();
        return preferences;
    }

    /// <summary>
    /// Get or create notification preferences for user
    /// </summary>
    private async Task<NotificationPreferences> GetNotificationPreferencesAsync(string userId)
    {
        var organizationId = await _userContext.GetActiveOrganizationIdAsync();

        var preferences = await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId
                && p.UserId == userId
                && !p.IsDeleted);

        if (preferences == null)
        {
            // Create default preferences
            preferences = new NotificationPreferences
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId!.Value,
                UserId = userId,
                EnableInAppNotifications = true,
                EnableEmailNotifications = true,
                EnableSMSNotifications = false,
                EmailLeaseExpiring = true,
                EmailPaymentDue = true,
                EmailPaymentReceived = true,
                EmailApplicationStatusChange = true,
                EmailMaintenanceUpdate = true,
                EmailInspectionScheduled = true
            };

            _context.NotificationPreferences.Add(preferences);
            await _context.SaveChangesAsync();
        }

        return preferences;
    }

    private bool ShouldSendEmail(string category, NotificationPreferences prefs)
    {
        return category switch
        {
            NotificationConstants.Categories.Lease => prefs.EmailLeaseExpiring,
            NotificationConstants.Categories.Payment => prefs.EmailPaymentDue,
            NotificationConstants.Categories.Application => prefs.EmailApplicationStatusChange,
            NotificationConstants.Categories.Maintenance => prefs.EmailMaintenanceUpdate,
            NotificationConstants.Categories.Inspection => prefs.EmailInspectionScheduled,
            _ => true
        };
    }

    private bool ShouldSendSMS(string category, NotificationPreferences prefs)
    {
        return category switch
        {
            NotificationConstants.Categories.Payment => prefs.SMSPaymentDue,
            NotificationConstants.Categories.Maintenance => prefs.SMSMaintenanceEmergency,
            NotificationConstants.Categories.Lease => prefs.SMSLeaseExpiringUrgent,
            _ => false
        };
    }
}