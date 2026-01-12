using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Aquiis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service responsible for generating and sending daily and weekly digest emails to users.
/// Consolidates activity metrics and notifications into scheduled email summaries.
/// </summary>
public class DigestService
{
    private readonly ILogger<DigestService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly NotificationService _notificationService;

    public DigestService(
        ILogger<DigestService> logger,
        ApplicationDbContext dbContext,
        NotificationService notificationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Sends daily digest emails to users who have opted in.
    /// Consolidates notifications and activity metrics from the last 24 hours.
    /// </summary>
    public async Task SendDailyDigestsAsync()
    {
        try
        {
            _logger.LogInformation("Starting daily digest email processing at {Time}", DateTime.Now);

            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);

            // Get all users with daily digest enabled
            var usersWithDigest = await _dbContext.NotificationPreferences
                .Where(np => np.EnableDailyDigest && 
                            !np.IsDeleted &&
                            np.EnableEmailNotifications &&
                            !string.IsNullOrEmpty(np.EmailAddress))
                .Include(np => np.Organization)
                .ToListAsync();

            if (!usersWithDigest.Any())
            {
                _logger.LogInformation("No users have daily digest enabled");
                return;
            }

            _logger.LogInformation("Processing daily digests for {Count} user(s)", usersWithDigest.Count);

            var successCount = 0;
            var failureCount = 0;

            // Process each user
            foreach (var userPrefs in usersWithDigest)
            {
                try
                {
                    // Get user's notifications from last 24 hours
                    var userNotifications = await _dbContext.Notifications
                        .Where(n => n.RecipientUserId == userPrefs.UserId &&
                                   n.OrganizationId == userPrefs.OrganizationId &&
                                   n.CreatedOn >= yesterday &&
                                   !n.IsDeleted)
                        .OrderByDescending(n => n.CreatedOn)
                        .Take(50) // Limit to 50 most recent
                        .ToListAsync();

                    var orgId = userPrefs.OrganizationId;

                    // Collect daily metrics
                    var newApplications = await _dbContext.RentalApplications
                        .Where(a => a.OrganizationId == orgId &&
                                   a.CreatedOn >= yesterday &&
                                   !a.IsDeleted)
                        .CountAsync();

                    var paymentsToday = await _dbContext.Payments
                        .Where(p => p.OrganizationId == orgId &&
                                   p.PaidOn.Date >= yesterday.Date &&
                                   !p.IsDeleted)
                        .ToListAsync();

                    var paymentsCount = paymentsToday.Count;
                    var paymentsTotal = paymentsToday.Sum(p => p.Amount);

                    var maintenanceCreated = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == orgId &&
                                   m.RequestedOn >= yesterday &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var maintenanceCompleted = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == orgId &&
                                   m.CompletedOn != null &&
                                   m.CompletedOn >= yesterday &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var inspectionsScheduled = await _dbContext.Inspections
                        .Where(i => i.OrganizationId == orgId &&
                                   i.CreatedOn >= yesterday &&
                                   !i.IsDeleted)
                        .CountAsync();

                    var inspectionsCompleted = await _dbContext.Inspections
                        .Where(i => i.OrganizationId == orgId &&
                                   i.CompletedOn != null &&
                                   i.CompletedOn >= yesterday &&
                                   !i.IsDeleted)
                        .CountAsync();

                    var leasesExpiringSoon = await _dbContext.Leases
                        .Where(l => l.OrganizationId == orgId &&
                                   l.EndDate <= DateTime.Today.AddDays(90) &&
                                   l.EndDate > DateTime.Today &&
                                   !l.IsDeleted)
                        .CountAsync();

                    var activeProperties = await _dbContext.Properties
                        .Where(p => p.OrganizationId == orgId &&
                                   !p.IsDeleted)
                        .CountAsync();

                    var occupiedProperties = await _dbContext.Properties
                        .Where(p => p.OrganizationId == orgId &&
                                   !p.IsDeleted &&
                                   p.Status == ApplicationConstants.PropertyStatuses.Occupied)
                        .CountAsync();

                    var outstandingInvoices = await _dbContext.Invoices
                        .Where(i => i.OrganizationId == orgId &&
                                   (i.Status == ApplicationConstants.InvoiceStatuses.Pending ||
                                    i.Status == ApplicationConstants.InvoiceStatuses.Overdue) &&
                                   !i.IsDeleted)
                        .SumAsync(i => i.Amount);

                    // Build email content
                    var subject = $"Daily Digest - {userPrefs.Organization?.Name ?? "Property Management"} - {DateTime.Today:MMM dd, yyyy}";
                    
                    var body = BuildDailyDigestEmailBody(
                        userPrefs.UserId,
                        userPrefs.Organization?.Name ?? "Property Management",
                        DateTime.Today.ToString("MMMM dd, yyyy"),
                        newApplications,
                        paymentsCount,
                        paymentsTotal,
                        maintenanceCreated,
                        maintenanceCompleted,
                        inspectionsScheduled,
                        inspectionsCompleted,
                        leasesExpiringSoon,
                        userNotifications,
                        activeProperties,
                        occupiedProperties,
                        outstandingInvoices);

                    // Send email via NotificationService's email service
                    await _notificationService.SendEmailDirectAsync(
                        userPrefs.EmailAddress!,
                        subject,
                        body);

                    successCount++;
                    _logger.LogInformation(
                        "Sent daily digest to {Email} for organization {OrgName} ({NotificationCount} notifications)",
                        userPrefs.EmailAddress,
                        userPrefs.Organization?.Name,
                        userNotifications.Count);
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex,
                        "Failed to send daily digest to user {UserId} in organization {OrgId}",
                        userPrefs.UserId,
                        userPrefs.OrganizationId);
                }
            }

            _logger.LogInformation(
                "Daily digest processing complete: {Success} sent, {Failures} failed",
                successCount,
                failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing daily digest emails");
        }
    }

    /// <summary>
    /// Builds the HTML body for the daily digest email
    /// </summary>
    private string BuildDailyDigestEmailBody(
        string userId,
        string organizationName,
        string date,
        int newApplications,
        int paymentsCount,
        decimal paymentsTotal,
        int maintenanceCreated,
        int maintenanceCompleted,
        int inspectionsScheduled,
        int inspectionsCompleted,
        int leasesExpiringSoon,
        List<Notification> notifications,
        int activeProperties,
        int occupiedProperties,
        decimal outstandingInvoices)
    {
        var notificationList = string.Empty;
        if (notifications.Any())
        {
            var notificationItems = notifications
                .Take(10) // Show top 10
                .Select(n =>
                {
                    var typeColor = n.Type switch
                    {
                        "Success" => "#10b981",
                        "Warning" => "#f59e0b",
                        "Error" => "#ef4444",
                        _ => "#6b7280"
                    };

                    return $@"
                    <div style='padding: 12px; border-left: 3px solid {typeColor}; margin-bottom: 8px; background-color: #f9fafb;'>
                        <div style='font-weight: 600; color: #111827; margin-bottom: 4px;'>{n.Title}</div>
                        <div style='color: #6b7280; font-size: 14px; margin-bottom: 4px;'>{n.Message}</div>
                        <div style='color: #9ca3af; font-size: 12px;'>{n.CreatedOn:MMM dd, HH:mm} · <span style='color: {typeColor};'>{n.Type}</span></div>
                    </div>";
                });

            notificationList = string.Join("", notificationItems);
        }
        else
        {
            notificationList = "<p style='color: #6b7280; text-align: center; padding: 20px;'>No new notifications in the last 24 hours</p>";
        }

        var occupancyRate = activeProperties > 0 ? (occupiedProperties * 100.0 / activeProperties) : 0;

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; line-height: 1.5; color: #1f2937; margin: 0; padding: 20px; background-color: #f3f4f6;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.1);'>
        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px 20px; text-align: center;'>
            <h1 style='margin: 0; font-size: 24px; font-weight: 600;'>Daily Digest</h1>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{organizationName}</p>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{date}</p>
        </div>

        <div style='padding: 20px;'>
            <h2 style='color: #111827; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                📊 Activity Overview (Last 24 Hours)
            </h2>
            
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>📝 New Applications</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{newApplications}</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>💰 Payments Received</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #10b981;'>{paymentsCount} (${paymentsTotal:N2})</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>🔧 Maintenance Requests</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{maintenanceCreated} new / {maintenanceCompleted} completed</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>🔍 Inspections</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{inspectionsScheduled} scheduled / {inspectionsCompleted} completed</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>⏰ Leases Expiring Soon</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>{leasesExpiringSoon} (within 90 days)</td>
                </tr>
            </table>

            <h2 style='color: #111827; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                🔔 Your Notifications
            </h2>
            <div style='margin-bottom: 25px;'>
                {notificationList}
            </div>

            <h2 style='color: #111827; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                📈 Quick Stats
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>Active Properties</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{activeProperties}</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>Occupancy Rate</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{occupancyRate:F1}% ({occupiedProperties}/{activeProperties})</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>Outstanding Invoices</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #ef4444;'>${outstandingInvoices:N2}</td>
                </tr>
            </table>
        </div>

        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
            <p style='margin: 0; color: #6b7280; font-size: 14px;'>
                You received this email because you have daily digest notifications enabled.
            </p>
            <p style='margin: 10px 0 0 0; color: #9ca3af; font-size: 12px;'>
                To manage your notification preferences, visit your account settings.
            </p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Sends weekly digest emails to users who have opted in.
    /// Consolidates 7-day metrics and provides weekly summary.
    /// </summary>
    public async Task SendWeeklyDigestsAsync()
    {
        try
        {
            _logger.LogInformation("Starting weekly digest email processing at {Time}", DateTime.Now);

            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;

            // Get all users with weekly digest enabled
            var usersWithWeeklyDigest = await _dbContext.NotificationPreferences
                .Where(np => np.EnableWeeklyDigest &&
                            !np.IsDeleted &&
                            np.EnableEmailNotifications &&
                            !string.IsNullOrEmpty(np.EmailAddress))
                .Include(np => np.Organization)
                .ToListAsync();

            if (!usersWithWeeklyDigest.Any())
            {
                _logger.LogInformation("No users have weekly digest enabled");
                return;
            }

            _logger.LogInformation("Sending weekly digests to {Count} users", usersWithWeeklyDigest.Count);

            int successCount = 0;
            int failureCount = 0;

            foreach (var pref in usersWithWeeklyDigest)
            {
                try
                {
                    var organizationId = pref.OrganizationId;
                    var userId = pref.UserId;
                    var organizationName = pref.Organization?.Name ?? "Property Management";

                    // Collect weekly metrics
                    var applicationsSubmitted = await _dbContext.RentalApplications
                        .Where(a => a.OrganizationId == organizationId &&
                                   a.CreatedOn >= startDate &&
                                   !a.IsDeleted)
                        .CountAsync();

                    var applicationsApproved = await _dbContext.RentalApplications
                        .Where(a => a.OrganizationId == organizationId &&
                                   a.LastModifiedOn >= startDate &&
                                   a.Status == ApplicationConstants.ApplicationStatuses.Approved &&
                                   !a.IsDeleted)
                        .CountAsync();

                    var applicationsDenied = await _dbContext.RentalApplications
                        .Where(a => a.OrganizationId == organizationId &&
                                   a.LastModifiedOn >= startDate &&
                                   a.Status == ApplicationConstants.ApplicationStatuses.Denied &&
                                   !a.IsDeleted)
                        .CountAsync();

                    var leasesCreated = await _dbContext.Leases
                        .Where(l => l.OrganizationId == organizationId &&
                                   l.StartDate >= startDate.Date &&
                                   !l.IsDeleted)
                        .CountAsync();

                    var leasesExpiring = await _dbContext.Leases
                        .Where(l => l.OrganizationId == organizationId &&
                                   l.EndDate >= DateTime.Today &&
                                   l.EndDate <= DateTime.Today.AddDays(30) &&
                                   !l.IsDeleted)
                        .CountAsync();

                    var weeklyPayments = await _dbContext.Payments
                        .Where(p => p.OrganizationId == organizationId &&
                                   p.PaidOn >= startDate &&
                                   !p.IsDeleted)
                        .ToListAsync();

                    var totalRevenue = weeklyPayments.Sum(p => p.Amount);
                    var paymentsReceived = weeklyPayments.Count;

                    var maintenanceCreated = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.RequestedOn >= startDate &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var maintenanceCompleted = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.CompletedOn != null &&
                                   m.CompletedOn >= startDate &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var maintenancePending = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var activeProperties = await _dbContext.Properties
                        .Where(p => p.OrganizationId == organizationId &&
                                   !p.IsDeleted)
                        .CountAsync();

                    var occupiedProperties = await _dbContext.Properties
                        .Where(p => p.OrganizationId == organizationId &&
                                   !p.IsDeleted &&
                                   p.Status == ApplicationConstants.PropertyStatuses.Occupied)
                        .CountAsync();

                    var occupancyRate = activeProperties > 0 ? (occupiedProperties * 100.0 / activeProperties) : 0;

                    var vacantOver30Days = await _dbContext.Properties
                        .Where(p => p.OrganizationId == organizationId &&
                                   p.Status == ApplicationConstants.PropertyStatuses.Available &&
                                   p.LastModifiedOn < DateTime.Now.AddDays(-30) &&
                                   !p.IsDeleted)
                        .CountAsync();

                    var overdueMaintenanceCount = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.ScheduledOn < DateTime.Now &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var overdueInvoicesOver30Days = await _dbContext.Invoices
                        .Where(i => i.OrganizationId == organizationId &&
                                   i.DueOn < DateTime.Now.AddDays(-30) &&
                                   (i.Status == ApplicationConstants.InvoiceStatuses.Pending ||
                                    i.Status == ApplicationConstants.InvoiceStatuses.Overdue) &&
                                   !i.IsDeleted)
                        .CountAsync();

                    // Build email
                    var emailBody = BuildWeeklyDigestEmailBody(
                        organizationName,
                        startDate,
                        endDate,
                        applicationsSubmitted,
                        applicationsApproved,
                        applicationsDenied,
                        leasesCreated,
                        leasesExpiring,
                        totalRevenue,
                        paymentsReceived,
                        maintenanceCreated,
                        maintenanceCompleted,
                        maintenancePending,
                        activeProperties,
                        occupancyRate,
                        vacantOver30Days,
                        overdueMaintenanceCount,
                        overdueInvoicesOver30Days
                    );

                    // Get user's email address from NotificationPreferences
                    var userEmail = pref.EmailAddress;
                    
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        // Send email
                        await _notificationService.SendEmailDirectAsync(
                            userEmail,
                            $"Weekly Digest: {organizationName} - {startDate:MMM dd} to {endDate:MMM dd}",
                            emailBody,
                            organizationName
                        );

                        successCount++;
                        _logger.LogInformation("Sent weekly digest to user {UserId}", userId);
                    }
                    else
                    {
                        _logger.LogWarning("User {UserId} has no email address configured for weekly digest", userId);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex, "Error sending weekly digest to user {UserId}", pref.UserId);
                }
            }

            _logger.LogInformation("Weekly digest summary: {Success} succeeded, {Failure} failed", 
                successCount, failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendWeeklyDigestsAsync");
            throw;
        }
    }

    /// <summary>
    /// Builds the HTML body for weekly digest emails
    /// </summary>
    private string BuildWeeklyDigestEmailBody(
        string organizationName,
        DateTime startDate,
        DateTime endDate,
        int applicationsSubmitted,
        int applicationsApproved,
        int applicationsDenied,
        int leasesCreated,
        int leasesExpiring,
        decimal totalRevenue,
        int paymentsReceived,
        int maintenanceCreated,
        int maintenanceCompleted,
        int maintenancePending,
        int activeProperties,
        double occupancyRate,
        int vacantOver30Days,
        int overdueMaintenanceCount,
        int overdueInvoicesOver30Days)
    {
        var needsAttention = string.Empty;
        if (overdueMaintenanceCount > 0 || vacantOver30Days > 0 || overdueInvoicesOver30Days > 0)
        {
            needsAttention = $@"
            <h2 style='color: #111827; font-size: 18px; margin: 25px 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                ⚠️ Needs Attention
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>";

            if (overdueMaintenanceCount > 0)
            {
                needsAttention += $@"
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #ef4444;'>Overdue Maintenance Requests</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #ef4444;'>{overdueMaintenanceCount}</td>
                </tr>";
            }

            if (vacantOver30Days > 0)
            {
                needsAttention += $@"
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #f59e0b;'>Properties Vacant >30 Days</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>{vacantOver30Days}</td>
                </tr>";
            }

            if (overdueInvoicesOver30Days > 0)
            {
                needsAttention += $@"
                <tr>
                    <td style='padding: 10px 0; color: #f59e0b;'>Invoices Overdue >30 Days</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>{overdueInvoicesOver30Days}</td>
                </tr>";
            }

            needsAttention += @"
            </table>";
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; line-height: 1.5; color: #1f2937; margin: 0; padding: 20px; background-color: #f3f4f6;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.1);'>
        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px 20px; text-align: center;'>
            <h1 style='margin: 0; font-size: 24px; font-weight: 600;'>Weekly Digest</h1>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{organizationName}</p>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{startDate:MMM dd} - {endDate:MMM dd, yyyy}</p>
        </div>

        <div style='padding: 20px;'>
            <h2 style='color: #111827; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                📊 Weekly Overview
            </h2>
            
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>📝 Applications</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{applicationsSubmitted} submitted, {applicationsApproved} approved, {applicationsDenied} denied</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>📄 Leases</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{leasesCreated} new, {leasesExpiring} expiring next 30 days</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>💰 Revenue</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #10b981;'>${totalRevenue:N2} collected ({paymentsReceived} payments)</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>🔧 Maintenance</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{maintenanceCreated} new, {maintenanceCompleted} resolved, {maintenancePending} pending</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>🏠 Occupancy</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{occupancyRate:F1}% ({activeProperties} properties)</td>
                </tr>
            </table>

            {needsAttention}
        </div>

        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
            <p style='margin: 0; color: #6b7280; font-size: 14px;'>
                You received this email because you have weekly digest notifications enabled.
            </p>
            <p style='margin: 10px 0 0 0; color: #9ca3af; font-size: 12px;'>
                To manage your notification preferences, visit your account settings.
            </p>
        </div>
    </div>
</body>
</html>";
    }
}
