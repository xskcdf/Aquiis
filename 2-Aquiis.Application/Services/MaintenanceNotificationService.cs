using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Aquiis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service responsible for generating and sending maintenance status summaries.
/// Provides weekly reports on maintenance request metrics, pending work, and alerts.
/// </summary>
public class MaintenanceNotificationService
{
    private readonly ILogger<MaintenanceNotificationService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly NotificationService _notificationService;

    public MaintenanceNotificationService(
        ILogger<MaintenanceNotificationService> logger,
        ApplicationDbContext dbContext,
        NotificationService notificationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Sends weekly maintenance status summary emails to property managers.
    /// Includes weekly statistics, pending requests by priority, and alerts.
    /// </summary>
    public async Task SendMaintenanceStatusSummaryAsync()
    {
        try
        {
            _logger.LogInformation("Starting maintenance status summary email processing at {Time}", DateTime.Now);

            var now = DateTime.Now;
            var startDate = now.AddDays(-7);

            // Get all users with weekly digest enabled (maintenance summary is part of weekly digest)
            var usersWithWeeklyDigest = await _dbContext.NotificationPreferences
                .Where(np => np.EnableWeeklyDigest &&
                            !np.IsDeleted &&
                            np.EnableEmailNotifications &&
                            !string.IsNullOrEmpty(np.EmailAddress))
                .Include(np => np.Organization)
                .ToListAsync();

            if (!usersWithWeeklyDigest.Any())
            {
                _logger.LogInformation("No users have weekly digest enabled for maintenance summary");
                return;
            }

            _logger.LogInformation("Sending maintenance status summary to {Count} users", usersWithWeeklyDigest.Count);

            int successCount = 0;
            int failureCount = 0;

            foreach (var pref in usersWithWeeklyDigest)
            {
                try
                {
                    var organizationId = pref.OrganizationId;
                    var organizationName = pref.Organization?.Name ?? "Property Management";

                    // Weekly maintenance statistics
                    var weeklySubmitted = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.RequestedOn >= startDate &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var weeklyCompleted = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.CompletedOn >= startDate &&
                                   m.Status == ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var completedRequests = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.CompletedOn != null &&
                                   m.CompletedOn >= startDate &&
                                   m.Status == ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   !m.IsDeleted)
                        .Select(m => new
                        {
                            ResolutionTime = (m.CompletedOn!.Value - m.RequestedOn).Days
                        })
                        .ToListAsync();

                    var avgResolutionDays = completedRequests.Any()
                        ? completedRequests.Average(r => r.ResolutionTime)
                        : 0;

                    // Pending requests by priority
                    var urgentPending = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   m.Priority == ApplicationConstants.MaintenanceRequestPriorities.Urgent &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var highPending = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   m.Priority == ApplicationConstants.MaintenanceRequestPriorities.High &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var mediumPending = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   m.Priority == ApplicationConstants.MaintenanceRequestPriorities.Medium &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var lowPending = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   m.Priority == ApplicationConstants.MaintenanceRequestPriorities.Low &&
                                   !m.IsDeleted)
                        .CountAsync();

                    // Alerts
                    var overdueCount = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.ScheduledOn < now &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   !m.IsDeleted)
                        .CountAsync();

                    var unassignedCount = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status == ApplicationConstants.MaintenanceRequestStatuses.Submitted &&
                                   !m.IsDeleted)
                        .CountAsync();

                    // Top 5 properties by maintenance requests
                    var topProperties = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.RequestedOn >= startDate &&
                                   m.PropertyId != null &&
                                   !m.IsDeleted)
                        .GroupBy(m => new { m.PropertyId, m.Property!.Address })
                        .Select(g => new
                        {
                            PropertyAddress = g.Key.Address,
                            RequestCount = g.Count()
                        })
                        .OrderByDescending(p => p.RequestCount)
                        .Take(5)
                        .ToListAsync();

                    // Financial summary
                    var weeklyMaintenanceCost = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.RequestedOn >= startDate &&
                                   m.EstimatedCost != null &&
                                   !m.IsDeleted)
                        .SumAsync(m => m.EstimatedCost);

                    var totalPendingCost = await _dbContext.MaintenanceRequests
                        .Where(m => m.OrganizationId == organizationId &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                   m.Status != ApplicationConstants.MaintenanceRequestStatuses.Cancelled &&
                                   m.EstimatedCost != null &&
                                   !m.IsDeleted)
                        .SumAsync(m => m.EstimatedCost);

                    // Build email
                    var emailBody = BuildMaintenanceStatusSummaryEmailBody(
                        organizationName,
                        startDate,
                        now,
                        weeklySubmitted,
                        weeklyCompleted,
                        avgResolutionDays,
                        urgentPending,
                        highPending,
                        mediumPending,
                        lowPending,
                        overdueCount,
                        unassignedCount,
                        topProperties.Select(p => (p.PropertyAddress ?? "Unknown", p.RequestCount)).ToList(),
                        weeklyMaintenanceCost,
                        totalPendingCost
                    );

                    var userEmail = pref.EmailAddress;
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        await _notificationService.SendEmailDirectAsync(
                            userEmail,
                            $"Maintenance Status Summary: {organizationName} - {startDate:MMM dd} to {now:MMM dd}",
                            emailBody,
                            organizationName
                        );

                        successCount++;
                        _logger.LogInformation("Sent maintenance summary to user {UserId}", pref.UserId);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex, "Error sending maintenance summary to user {UserId}", pref.UserId);
                }
            }

            _logger.LogInformation(
                "Maintenance status summary complete: {Success} sent, {Failure} failed",
                successCount,
                failureCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendMaintenanceStatusSummaryAsync");
        }
    }

    /// <summary>
    /// Builds the HTML body for maintenance status summary emails
    /// </summary>
    private string BuildMaintenanceStatusSummaryEmailBody(
        string organizationName,
        DateTime startDate,
        DateTime endDate,
        int weeklySubmitted,
        int weeklyCompleted,
        double avgResolutionDays,
        int urgentPending,
        int highPending,
        int mediumPending,
        int lowPending,
        int overdueCount,
        int unassignedCount,
        List<(string address, int count)> topProperties,
        decimal weeklyMaintenanceCost,
        decimal totalPendingCost)
    {
        var topPropertiesHtml = string.Empty;
        if (topProperties.Any())
        {
            var propertyRows = topProperties.Select(p => $@"
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>{p.address}</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{p.count}</td>
                </tr>");

            topPropertiesHtml = $@"
            <h2 style='color: #111827; font-size: 18px; margin: 25px 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                🏆 Top Properties (Most Requests This Week)
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                {string.Join("", propertyRows)}
            </table>";
        }

        var alertsHtml = string.Empty;
        if (overdueCount > 0 || unassignedCount > 0)
        {
            alertsHtml = $@"
            <h2 style='color: #111827; font-size: 18px; margin: 25px 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                ⚠️ Alerts
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>";

            if (overdueCount > 0)
            {
                alertsHtml += $@"
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #ef4444;'>Overdue Requests</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #ef4444;'>{overdueCount}</td>
                </tr>";
            }

            if (unassignedCount > 0)
            {
                alertsHtml += $@"
                <tr>
                    <td style='padding: 10px 0; color: #f59e0b;'>Unassigned Requests</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>{unassignedCount}</td>
                </tr>";
            }

            alertsHtml += "</table>";
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
        <div style='background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); color: white; padding: 30px 20px; text-align: center;'>
            <h1 style='margin: 0; font-size: 24px; font-weight: 600;'>🔧 Maintenance Status Summary</h1>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{organizationName}</p>
            <p style='margin: 5px 0 0 0; font-size: 14px; opacity: 0.9;'>{startDate:MMM dd} - {endDate:MMM dd, yyyy}</p>
        </div>

        <div style='padding: 20px;'>
            <h2 style='color: #111827; font-size: 18px; margin: 0 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                📊 Weekly Statistics
            </h2>
            
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>New Requests Submitted</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{weeklySubmitted}</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>Requests Completed</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #10b981;'>{weeklyCompleted}</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>Avg Resolution Time</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>{avgResolutionDays:F1} days</td>
                </tr>
            </table>

            <h2 style='color: #111827; font-size: 18px; margin: 25px 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                📋 Pending Requests by Priority
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #ef4444;'>🔴 Urgent</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #ef4444;'>{urgentPending}</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #f59e0b;'>🟠 High</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>{highPending}</td>
                </tr>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #3b82f6;'>🟡 Medium</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #3b82f6;'>{mediumPending}</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>⚪ Low</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #6b7280;'>{lowPending}</td>
                </tr>
            </table>

            {alertsHtml}

            {topPropertiesHtml}

            <h2 style='color: #111827; font-size: 18px; margin: 25px 0 15px 0; border-bottom: 2px solid #e5e7eb; padding-bottom: 10px;'>
                💰 Financial Summary
            </h2>
            <table style='width: 100%; border-collapse: collapse; margin-bottom: 25px;'>
                <tr style='border-bottom: 1px solid #e5e7eb;'>
                    <td style='padding: 10px 0; color: #6b7280;'>Weekly Maintenance Cost</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #111827;'>${weeklyMaintenanceCost:N2}</td>
                </tr>
                <tr>
                    <td style='padding: 10px 0; color: #6b7280;'>Total Pending Cost</td>
                    <td style='padding: 10px 0; text-align: right; font-weight: 600; color: #f59e0b;'>${totalPendingCost:N2}</td>
                </tr>
            </table>
        </div>

        <div style='background-color: #f9fafb; padding: 20px; text-align: center; border-top: 1px solid #e5e7eb;'>
            <p style='margin: 0; color: #6b7280; font-size: 14px;'>
                This maintenance summary is part of your weekly digest.
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
