using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Aquiis.Application.Services.Workflows;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Aquiis.Application.Services
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly ILogger<ScheduledTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly NotificationService _notificationService;
        private Timer? _timer;
        private Timer? _dailyTimer;
        private Timer? _hourlyTimer;

        public ScheduledTaskService(
            ILogger<ScheduledTaskService> logger,
            IServiceProvider serviceProvider,
            NotificationService notificationService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service is starting.");

            // Run immediately on startup
            await DoWork(stoppingToken);

            // Then run daily at 2 AM
            _timer = new Timer(
                async _ => await DoWork(stoppingToken),
                null,
                TimeSpan.FromMinutes(GetMinutesUntil2AM()),
                TimeSpan.FromHours(24));

            await Task.CompletedTask;

            // Calculate time until next midnight for daily tasks
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            var timeUntilMidnight = nextMidnight - now;

            // Start daily timer (executes at midnight)
            _dailyTimer = new Timer(
                async _ => await ExecuteDailyTasks(),
                null,
                timeUntilMidnight,
                TimeSpan.FromDays(1));

            // Start hourly timer (executes every hour)
            _hourlyTimer = new Timer(
                async _ => await ExecuteHourlyTasks(),
                null,
                TimeSpan.Zero, // Start immediately
                TimeSpan.FromHours(1));

            _logger.LogInformation("Scheduled Task Service started. Daily tasks will run at midnight, hourly tasks every hour.");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Running scheduled tasks at {time}", DateTimeOffset.Now);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var organizationService = scope.ServiceProvider.GetRequiredService<OrganizationService>();

                    // Get all distinct organization IDs from OrganizationSettings
                    var organizations = await dbContext.OrganizationSettings
                        .Where(s => !s.IsDeleted)
                        .Select(s => s.OrganizationId)
                        .Distinct()
                        .ToListAsync(stoppingToken);

                    foreach (var organizationId in organizations)
                    {
                        // Get settings for this organization
                        var settings = await organizationService.GetOrganizationSettingsByOrgIdAsync(organizationId);
                        
                        if (settings == null)
                        {
                            _logger.LogWarning("No settings found for organization {OrganizationId}, skipping", organizationId);
                            continue;
                        }

                        // Task 1: Apply late fees to overdue invoices (if enabled)
                        if (settings.LateFeeEnabled && settings.LateFeeAutoApply)
                        {
                            await ApplyLateFees(dbContext, organizationId, settings, stoppingToken);
                        }

                        // Task 2: Update invoice statuses
                        await UpdateInvoiceStatuses(dbContext, organizationId, stoppingToken);

                        // Task 3: Send payment reminders (if enabled)
                        if (settings.PaymentReminderEnabled)
                        {
                            await SendPaymentReminders(dbContext, organizationId, settings, stoppingToken);
                        }

                        // Task 4: Check for expiring leases and send renewal notifications
                        await CheckLeaseRenewals(dbContext, organizationId, stoppingToken);

                        // Task 5: Expire overdue leases using workflow service (with audit logging)
                        var expiredLeaseCount = await ExpireOverdueLeases(scope, organizationId);
                        if (expiredLeaseCount > 0)
                        {
                            _logger.LogInformation(
                                "Expired {Count} overdue lease(s) for organization {OrganizationId}",
                                expiredLeaseCount, organizationId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing scheduled tasks.");
            }
        }

        private async Task ApplyLateFees(
            ApplicationDbContext dbContext, 
            Guid organizationId,
            OrganizationSettings settings,
            CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;

                // Find overdue invoices that haven't been charged a late fee yet
                var overdueInvoices = await dbContext.Invoices
                    .Include(i => i.Lease)
                    .Where(i => !i.IsDeleted &&
                               i.OrganizationId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueOn < today.AddDays(-settings.LateFeeGracePeriodDays) &&
                               (i.LateFeeApplied == null || !i.LateFeeApplied.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in overdueInvoices)
                {
                    var lateFee = Math.Min(invoice.Amount * settings.LateFeePercentage, settings.MaxLateFeeAmount);
                    
                    invoice.LateFeeAmount = lateFee;
                    invoice.LateFeeApplied = true;
                    invoice.LateFeeAppliedOn = DateTime.UtcNow;
                    invoice.Amount += lateFee;
                    invoice.Status = "Overdue";
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task
                    invoice.Notes = string.IsNullOrEmpty(invoice.Notes)
                        ? $"Late fee of {lateFee:C} applied on {DateTime.Now:MMM dd, yyyy}"
                        : $"{invoice.Notes}\nLate fee of {lateFee:C} applied on {DateTime.Now:MMM dd, yyyy}";

                    _logger.LogInformation(
                        "Applied late fee of {LateFee:C} to invoice {InvoiceNumber} (ID: {InvoiceId}) for organization {OrganizationId}",
                        lateFee, invoice.InvoiceNumber, invoice.Id, organizationId);
                }

                if (overdueInvoices.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Applied late fees to {Count} invoices for organization {OrganizationId}", 
                        overdueInvoices.Count, organizationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying late fees for organization {OrganizationId}", organizationId);
            }
        }

        private async Task UpdateInvoiceStatuses(ApplicationDbContext dbContext, Guid organizationId, CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;

                // Update pending invoices that are now overdue (and haven't had late fees applied)
                var newlyOverdueInvoices = await dbContext.Invoices
                    .Where(i => !i.IsDeleted &&
                               i.OrganizationId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueOn < today &&
                               (i.LateFeeApplied == null || !i.LateFeeApplied.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in newlyOverdueInvoices)
                {
                    invoice.Status = "Overdue";
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task
                }

                if (newlyOverdueInvoices.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Updated {Count} invoices to Overdue status for organization {OrganizationId}", 
                        newlyOverdueInvoices.Count, organizationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice statuses for organization {OrganizationId}", organizationId);
            }
        }

        private async Task SendPaymentReminders(
            ApplicationDbContext dbContext, 
            Guid organizationId,
            OrganizationSettings settings,
            CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;

                // Find invoices due soon
                var upcomingInvoices = await dbContext.Invoices
                    .Include(i => i.Lease)
                        .ThenInclude(l => l.Tenant)
                    .Include(i => i.Lease)
                        .ThenInclude(l => l.Property)
                    .Where(i => !i.IsDeleted &&
                               i.OrganizationId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueOn >= today &&
                               i.DueOn <= today.AddDays(settings.PaymentReminderDaysBefore) &&
                               (i.ReminderSent == null || !i.ReminderSent.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in upcomingInvoices)
                {
                    // TODO: Integrate with email service when implemented
                    // For now, just log the reminder
                    _logger.LogInformation(
                        "Payment reminder needed for invoice {InvoiceNumber} due {DueDate} for tenant {TenantName} in organization {OrganizationId}",
                        invoice.InvoiceNumber,
                        invoice.DueOn.ToString("MMM dd, yyyy"),
                        invoice.Lease?.Tenant?.FullName ?? "Unknown",
                        organizationId);

                    invoice.ReminderSent = true;
                    invoice.ReminderSentOn = DateTime.UtcNow;
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task
                }

                if (upcomingInvoices.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Marked {Count} invoices as reminder sent for organization {OrganizationId}", 
                        upcomingInvoices.Count, organizationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment reminders for organization {OrganizationId}", organizationId);
            }
        }

        private async Task CheckLeaseRenewals(ApplicationDbContext dbContext, Guid organizationId, CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;
                var addresses = string.Empty;

                // Check for leases expiring in 90 days (initial notification)
                var leasesExpiring90Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.OrganizationId == organizationId &&
                               l.Status == "Active" &&
                               l.EndDate >= today.AddDays(85) &&
                               l.EndDate <= today.AddDays(95) &&
                               (l.RenewalNotificationSent == null || !l.RenewalNotificationSent.Value))
                    .ToListAsync(stoppingToken);

                foreach (var lease in leasesExpiring90Days)
                {
                    // TODO: Send email notification when email service is integrated
                    _logger.LogInformation(
                        "Lease expiring in 90 days: Lease ID {LeaseId}, Property: {PropertyAddress}, Tenant: {TenantName}, End Date: {EndDate}",
                        lease.Id,
                        lease.Property?.Address ?? "Unknown",
                        lease.Tenant?.FullName ?? "Unknown",
                        lease.EndDate.ToString("MMM dd, yyyy"));

                    lease.RenewalNotificationSent = true;
                    lease.RenewalNotificationSentOn = DateTime.UtcNow;
                    lease.RenewalStatus = "Pending";
                    lease.LastModifiedOn = DateTime.UtcNow;
                    lease.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task

                    addresses += lease.Property?.Address + "\n";
                }

                // Send organization-wide notification if any leases are expiring
                await _notificationService.NotifyAllUsersAsync(
                    organizationId,
                    "90-Day Lease Renewal Notification",
                    $"The following properties have leases expiring in 90 days:\n\n{addresses}",
                    NotificationConstants.Types.Info,
                    NotificationConstants.Categories.Lease,
                    null,
                    ApplicationConstants.EntityTypes.Lease);

                // clear addresses for next use
                addresses = string.Empty;

                // Check for leases expiring in 60 days (reminder)
                var leasesExpiring60Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.OrganizationId == organizationId &&
                               l.Status == "Active" &&
                               l.EndDate >= today.AddDays(55) &&
                               l.EndDate <= today.AddDays(65) &&
                               l.RenewalNotificationSent == true &&
                               l.RenewalReminderSentOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var lease in leasesExpiring60Days)
                {
                    // TODO: Send reminder email
                    _logger.LogInformation(
                        "Lease expiring in 60 days (reminder): Lease ID {LeaseId}, Property: {PropertyAddress}, Tenant: {TenantName}, End Date: {EndDate}",
                        lease.Id,
                        lease.Property?.Address ?? "Unknown",
                        lease.Tenant?.FullName ?? "Unknown",
                        lease.EndDate.ToString("MMM dd, yyyy"));

                    lease.RenewalReminderSentOn = DateTime.UtcNow;
                    lease.LastModifiedOn = DateTime.UtcNow;
                    lease.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task

                    addresses += lease.Property?.Address + "\n";
                }

                 // Send organization-wide notification if any leases are expiring
                await _notificationService.NotifyAllUsersAsync(
                    organizationId,
                    "60-Day Lease Renewal Notification",
                    $"The following properties have leases expiring in 60 days:\n\n{addresses}",
                    NotificationConstants.Types.Info,
                    NotificationConstants.Categories.Lease,
                    null,
                    ApplicationConstants.EntityTypes.Lease);

                // clear addresses for next use
                addresses = string.Empty;

                // Check for leases expiring in 30 days (final reminder)
                var leasesExpiring30Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.OrganizationId == organizationId &&
                               l.Status == "Active" &&
                               l.EndDate >= today.AddDays(25) &&
                               l.EndDate <= today.AddDays(35) &&
                               l.RenewalStatus == "Pending")
                    .ToListAsync(stoppingToken);

                foreach (var lease in leasesExpiring30Days)
                {
                    // TODO: Send final reminder
                    _logger.LogInformation(
                        "Lease expiring in 30 days (final reminder): Lease ID {LeaseId}, Property: {PropertyAddress}, Tenant: {TenantName}, End Date: {EndDate}",
                        lease.Id,
                        lease.Property?.Address ?? "Unknown",
                        lease.Tenant?.FullName ?? "Unknown",
                        lease.EndDate.ToString("MMM dd, yyyy"));

                    addresses += lease.Property?.Address + "\n";
                }

                // Send organization-wide notification if any leases are expiring
                await _notificationService.NotifyAllUsersAsync(
                    organizationId,
                    "30-Day Lease Renewal Notification",
                    $"The following properties have leases expiring in 30 days:\n\n{addresses}",
                    NotificationConstants.Types.Info,
                    NotificationConstants.Categories.Lease,
                    null,
                    ApplicationConstants.EntityTypes.Lease);

                // clear addresses for next use
                addresses = string.Empty;

                // Note: Lease expiration is now handled by ExpireOverdueLeases() 
                // which uses LeaseWorkflowService for proper audit logging

                var totalUpdated = leasesExpiring90Days.Count + leasesExpiring60Days.Count + 
                                  leasesExpiring30Days.Count;

                if (totalUpdated > 0)
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(
                        "Processed {Count} lease renewal notifications for organization {OrganizationId}: {Initial} initial, {Reminder60} 60-day, {Reminder30} 30-day reminders",
                        totalUpdated,
                        organizationId,
                        leasesExpiring90Days.Count,
                        leasesExpiring60Days.Count,
                        leasesExpiring30Days.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking lease renewals for organization {OrganizationId}", organizationId);
            }
        }

        private async Task ExecuteDailyTasks()
        {
            _logger.LogInformation("Executing daily tasks at {Time}", DateTime.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<PaymentService>();
                var propertyService = scope.ServiceProvider.GetRequiredService<PropertyService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Calculate daily payment totals
                var today = DateTime.Today;
                var todayPayments = await paymentService.GetAllAsync();
                var dailyTotal = todayPayments
                    .Where(p => p.PaidOn.Date == today && !p.IsDeleted)
                    .Sum(p => p.Amount);

                _logger.LogInformation("Daily Payment Total for {Date}: ${Amount:N2}", 
                    today.ToString("yyyy-MM-dd"), 
                    dailyTotal);

                // Check for overdue routine inspections
                var overdueInspections = await propertyService.GetPropertiesWithOverdueInspectionsAsync();
                if (overdueInspections.Any())
                {
                    _logger.LogWarning("{Count} properties have overdue routine inspections", 
                        overdueInspections.Count);
                    
                    foreach (var property in overdueInspections.Take(5)) // Log first 5
                    {
                        var daysOverdue = (DateTime.Today - property.NextRoutineInspectionDueDate!.Value).Days;
                        _logger.LogWarning("Property {Address} - Inspection overdue by {Days} days (Due: {DueDate})",
                            property.Address,
                            daysOverdue,
                            property.NextRoutineInspectionDueDate.Value.ToString("yyyy-MM-dd"));
                    }
                }

                // Check for inspections due soon (within 30 days)
                var dueSoonInspections = await propertyService.GetPropertiesWithInspectionsDueSoonAsync(30);
                if (dueSoonInspections.Any())
                {
                    _logger.LogInformation("{Count} propert(ies) have routine inspections due within 30 days", 
                        dueSoonInspections.Count);
                }

                // Check for expired rental applications
                var expiredApplicationsCount = await ExpireOldApplications(dbContext);
                if (expiredApplicationsCount > 0)
                {
                    _logger.LogInformation("Expired {Count} rental application(s) that passed their expiration date", 
                        expiredApplicationsCount);
                }

                // Check for expired lease offers (uses workflow service for audit logging)
                var expiredLeaseOffersCount = await ExpireOldLeaseOffers(scope);
                if (expiredLeaseOffersCount > 0)
                {
                    _logger.LogInformation("Expired {Count} lease offer(s) that passed their expiration date", 
                        expiredLeaseOffersCount);
                }

                // Check for year-end dividend calculation (runs in first week of January)
                if (today.Month == 1 && today.Day <= 7)
                {
                    await ProcessYearEndDividends(scope, today.Year - 1);
                }

                // Send daily digest emails to users who have opted in
                await SendDailyDigestsAsync(dbContext);

                // Additional daily tasks:
                // - Generate daily reports
                // - Send payment reminders
                // - Check for overdue invoices
                // - Archive old records
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing daily tasks");
            }
        }

        /// <summary>
        /// Sends daily digest emails to users who have opted in.
        /// Consolidates notifications and activity metrics from the last 24 hours.
        /// </summary>
        private async Task SendDailyDigestsAsync(ApplicationDbContext dbContext)
        {
            try
            {
                _logger.LogInformation("Starting daily digest email processing at {Time}", DateTime.Now);

                var now = DateTime.UtcNow;
                var yesterday = now.AddDays(-1);

                // Get all users with daily digest enabled
                var usersWithDigest = await dbContext.NotificationPreferences
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
                        var userNotifications = await dbContext.Notifications
                            .Where(n => n.RecipientUserId == userPrefs.UserId &&
                                       n.OrganizationId == userPrefs.OrganizationId &&
                                       n.CreatedOn >= yesterday &&
                                       !n.IsDeleted)
                            .OrderByDescending(n => n.CreatedOn)
                            .Take(50) // Limit to 50 most recent
                            .ToListAsync();

                        // Get daily metrics for user's organization
                        var orgId = userPrefs.OrganizationId;

                        // Applications submitted today
                        var newApplications = await dbContext.RentalApplications
                            .Where(a => a.OrganizationId == orgId &&
                                       a.CreatedOn >= yesterday &&
                                       !a.IsDeleted)
                            .CountAsync();

                        // Payments received today
                        var paymentsToday = await dbContext.Payments
                            .Where(p => p.OrganizationId == orgId &&
                                       p.PaidOn >= yesterday &&
                                       !p.IsDeleted)
                            .ToListAsync();
                        var paymentsCount = paymentsToday.Count;
                        var paymentsTotal = paymentsToday.Sum(p => p.Amount);

                        // Maintenance requests created/completed today
                        var maintenanceCreated = await dbContext.MaintenanceRequests
                            .Where(m => m.OrganizationId == orgId &&
                                       m.CreatedOn >= yesterday &&
                                       !m.IsDeleted)
                            .CountAsync();

                        var maintenanceCompleted = await dbContext.MaintenanceRequests
                            .Where(m => m.OrganizationId == orgId &&
                                       m.CompletedOn >= yesterday &&
                                       m.Status == ApplicationConstants.MaintenanceRequestStatuses.Completed &&
                                       !m.IsDeleted)
                            .CountAsync();

                        // Inspections completed today
                        var inspectionsScheduled = 0; // Note: Inspection entity doesn't have ScheduledDate - would need to query CalendarEvent

                        var inspectionsCompleted = await dbContext.Inspections
                            .Where(i => i.OrganizationId == orgId &&
                                       i.CompletedOn >= yesterday &&
                                       !i.IsDeleted)
                            .CountAsync();

                        // Leases expiring within 90 days (for context)
                        var leasesExpiringSoon = await dbContext.Leases
                            .Where(l => l.OrganizationId == orgId &&
                                       l.EndDate <= DateTime.Today.AddDays(90) &&
                                       l.EndDate >= DateTime.Today &&
                                       l.Status == ApplicationConstants.LeaseStatuses.Active &&
                                       !l.IsDeleted)
                            .CountAsync();

                        // Quick stats
                        var activeProperties = await dbContext.Properties
                            .Where(p => p.OrganizationId == orgId &&
                                       !p.IsDeleted)
                            .CountAsync();

                        var occupiedProperties = await dbContext.Properties
                            .Where(p => p.OrganizationId == orgId &&
                                       !p.IsDeleted &&
                                       p.Status == ApplicationConstants.PropertyStatuses.Occupied)
                            .CountAsync();

                        var outstandingInvoices = await dbContext.Invoices
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
                    .Select(n => $"<li><strong>{n.Title}</strong><br/><span style='color: #666; font-size: 14px;'>{n.Message.Substring(0, Math.Min(100, n.Message.Length))}{(n.Message.Length > 100 ? "..." : "")}</span><br/><span style='color: #999; font-size: 12px;'>{n.CreatedOn:MMM dd, h:mm tt}</span></li>")
                    .ToList();
                notificationList = string.Join("", notificationItems);
            }
            else
            {
                notificationList = "<li style='color: #999;'>No new notifications in the last 24 hours</li>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Daily Digest</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;'>
        <h1 style='color: #2c3e50; margin: 0 0 10px 0;'>Daily Digest</h1>
        <p style='color: #666; margin: 0;'>{organizationName} - {date}</p>
    </div>

    <div style='background-color: #fff; padding: 20px; border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 20px;'>
        <h2 style='color: #2c3e50; margin-top: 0;'>📊 Activity Overview</h2>
        <ul style='list-style: none; padding: 0;'>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>{newApplications}</strong> new rental application{(newApplications != 1 ? "s" : "")}</li>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>{paymentsCount}</strong> payment{(paymentsCount != 1 ? "s" : "")} received (<strong>${paymentsTotal:N2}</strong> total)</li>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>{maintenanceCreated}</strong> maintenance request{(maintenanceCreated != 1 ? "s" : "")} created, <strong>{maintenanceCompleted}</strong> completed</li>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'><strong>{inspectionsScheduled}</strong> inspection{(inspectionsScheduled != 1 ? "s" : "")} scheduled, <strong>{inspectionsCompleted}</strong> completed</li>
            <li style='padding: 8px 0;'><strong>{leasesExpiringSoon}</strong> lease{(leasesExpiringSoon != 1 ? "s" : "")} expiring within 90 days</li>
        </ul>
    </div>

    <div style='background-color: #fff; padding: 20px; border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 20px;'>
        <h2 style='color: #2c3e50; margin-top: 0;'>🔔 Your Notifications (Last 24 hours)</h2>
        <ul style='padding-left: 20px;'>
            {notificationList}
        </ul>
        {(notifications.Count > 10 ? $"<p style='color: #666; font-size: 14px; margin: 10px 0 0 0;'>...and {notifications.Count - 10} more notification{(notifications.Count - 10 != 1 ? "s" : "")}</p>" : "")}
    </div>

    <div style='background-color: #fff; padding: 20px; border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 20px;'>
        <h2 style='color: #2c3e50; margin-top: 0;'>📈 Quick Stats</h2>
        <ul style='list-style: none; padding: 0;'>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'>Active properties: <strong>{activeProperties}</strong></li>
            <li style='padding: 8px 0; border-bottom: 1px solid #eee;'>Occupied units: <strong>{occupiedProperties}</strong>/{activeProperties} ({(activeProperties > 0 ? (occupiedProperties * 100.0 / activeProperties).ToString("F1") : "0")}%)</li>
            <li style='padding: 8px 0;'>Outstanding invoices: <strong>${outstandingInvoices:N2}</strong></li>
        </ul>
    </div>

    <div style='text-align: center; padding: 20px; color: #999; font-size: 14px;'>
        <p>You're receiving this email because you've enabled daily digest notifications.</p>
        <p>To change your notification preferences, log in to your account and visit Settings.</p>
    </div>
</body>
</html>";
        }

        private async Task ExecuteHourlyTasks()
        {
            _logger.LogInformation("Executing hourly tasks at {Time}", DateTime.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var tourService = scope.ServiceProvider.GetRequiredService<TourService>();
                var leaseService = scope.ServiceProvider.GetRequiredService<LeaseService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get all organizations
                var organizations = await dbContext.OrganizationSettings
                    .Where(s => !s.IsDeleted)
                    .ToListAsync();

                int totalMarkedNoShow = 0;

                foreach (var orgSettings in organizations)
                {
                    var organizationId = orgSettings.OrganizationId;
                    var gracePeriodHours = orgSettings.TourNoShowGracePeriodHours;

                    // Check for tours that should be marked as no-show
                    var cutoffTime = DateTime.Now.AddHours(-gracePeriodHours);
                    
                    // Query tours directly for this organization (bypass user context)
                    var potentialNoShowTours = await dbContext.Tours
                        .Where(t => t.OrganizationId == organizationId && !t.IsDeleted)
                        .Include(t => t.ProspectiveTenant)
                        .Include(t => t.Property)
                        .ToListAsync();
                    
                    var noShowTours = potentialNoShowTours
                        .Where(t => t.Status == ApplicationConstants.TourStatuses.Scheduled &&
                                   t.ScheduledOn < cutoffTime)
                        .ToList();

                    foreach (var tour in noShowTours)
                    {
                        await tourService.MarkTourAsNoShowAsync(tour.Id);
                        totalMarkedNoShow++;
                        
                        _logger.LogInformation(
                            "Marked tour {TourId} as No Show - Scheduled: {ScheduledTime}, Grace period: {Hours} hours",
                            tour.Id,
                            tour.ScheduledOn.ToString("yyyy-MM-dd HH:mm"),
                            gracePeriodHours);
                    }
                }

                if (totalMarkedNoShow > 0)
                {
                    _logger.LogInformation("Marked {Count} tour(s) as No Show across all organizations", totalMarkedNoShow);
                }

                // Example hourly task: Check for upcoming lease expirations
                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userId))
                {
                    var upcomingLeases = await leaseService.GetAllAsync();
                    var expiringIn30Days = upcomingLeases
                        .Where(l => l.EndDate >= DateTime.Today && 
                                   l.EndDate <= DateTime.Today.AddDays(30) && 
                                   !l.IsDeleted)
                        .Count();

                    if (expiringIn30Days > 0)
                    {
                        _logger.LogInformation("{Count} lease(s) expiring in the next 30 days", expiringIn30Days);
                    }
                }

                // You can add more hourly tasks here:
                // - Check for maintenance requests
                // - Update lease statuses
                // - Send notifications
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing hourly tasks");
            }
        }

        private double GetMinutesUntil2AM()
        {
            var now = DateTime.Now;
            var next2AM = DateTime.Today.AddDays(1).AddHours(2);
            
            if (now.Hour < 2)
            {
                next2AM = DateTime.Today.AddHours(2);
            }

            return (next2AM - now).TotalMinutes;
        }

        private async Task<int> ExpireOldApplications(ApplicationDbContext dbContext)
        {
            try
            {
                // Find all applications that are expired but not yet marked as such
                var expiredApplications = await dbContext.RentalApplications
                    .Where(a => !a.IsDeleted &&
                               (a.Status == ApplicationConstants.ApplicationStatuses.Submitted ||
                                a.Status == ApplicationConstants.ApplicationStatuses.UnderReview ||
                                a.Status == ApplicationConstants.ApplicationStatuses.Screening) &&
                               a.ExpiresOn.HasValue &&
                               a.ExpiresOn.Value < DateTime.UtcNow)
                    .ToListAsync();

                foreach (var application in expiredApplications)
                {
                    application.Status = ApplicationConstants.ApplicationStatuses.Expired;
                    application.LastModifiedOn = DateTime.UtcNow;
                    application.LastModifiedBy = ApplicationConstants.SystemUser.Id; // Automated task
                    
                    _logger.LogInformation("Expired application {ApplicationId} for property {PropertyId} (Expired on: {ExpirationDate})",
                        application.Id,
                        application.PropertyId,
                        application.ExpiresOn!.Value.ToString("yyyy-MM-dd"));
                }

                if (expiredApplications.Any())
                {
                    await dbContext.SaveChangesAsync();
                }

                return expiredApplications.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring old applications");
                return 0;
            }
        }

        /// <summary>
        /// Expires lease offers that have passed their expiration date.
        /// Uses ApplicationWorkflowService for proper audit logging.
        /// </summary>
        private async Task<int> ExpireOldLeaseOffers(IServiceScope scope)
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var workflowService = scope.ServiceProvider.GetRequiredService<ApplicationWorkflowService>();

                // Find all pending lease offers that have expired
                var expiredOffers = await dbContext.LeaseOffers
                    .Where(lo => !lo.IsDeleted &&
                                lo.Status == "Pending" &&
                                lo.ExpiresOn < DateTime.UtcNow)
                    .ToListAsync();

                var expiredCount = 0;

                foreach (var offer in expiredOffers)
                {
                    try
                    {
                        var result = await workflowService.ExpireLeaseOfferAsync(offer.Id);
                        
                        if (result.Success)
                        {
                            expiredCount++;
                            _logger.LogInformation(
                                "Expired lease offer {LeaseOfferId} for property {PropertyId} (Expired on: {ExpirationDate})",
                                offer.Id,
                                offer.PropertyId,
                                offer.ExpiresOn.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Failed to expire lease offer {LeaseOfferId}: {Errors}",
                                offer.Id,
                                string.Join(", ", result.Errors));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error expiring lease offer {LeaseOfferId}", offer.Id);
                    }
                }

                return expiredCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring old lease offers");
                return 0;
            }
        }

        /// <summary>
        /// Processes year-end security deposit dividend calculations.
        /// Runs in the first week of January for the previous year.
        /// </summary>
        private async Task ProcessYearEndDividends(IServiceScope scope, int year)
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var securityDepositService = scope.ServiceProvider.GetRequiredService<SecurityDepositService>();

                // Get all organizations that have security deposit investment enabled
                var organizations = await dbContext.OrganizationSettings
                    .Where(s => !s.IsDeleted && s.SecurityDepositInvestmentEnabled)
                    .Select(s => s.OrganizationId)
                    .Distinct()
                    .ToListAsync();

                foreach (var organizationId in organizations)
                {
                    try
                    {
                        // Check if pool exists and has performance recorded
                        var pool = await dbContext.SecurityDepositInvestmentPools
                            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId &&
                                                     p.Year == year &&
                                                     !p.IsDeleted);

                        if (pool == null)
                        {
                            _logger.LogInformation(
                                "No investment pool found for organization {OrganizationId} for year {Year}",
                                organizationId, year);
                            continue;
                        }

                        if (pool.Status == "Distributed" || pool.Status == "Closed")
                        {
                            _logger.LogInformation(
                                "Dividends already processed for organization {OrganizationId} for year {Year}",
                                organizationId, year);
                            continue;
                        }

                        if (pool.TotalEarnings == 0)
                        {
                            _logger.LogInformation(
                                "No earnings recorded for organization {OrganizationId} for year {Year}. " +
                                "Please record investment performance before dividend calculation.",
                                organizationId, year);
                            continue;
                        }

                        // Calculate dividends
                        var dividends = await securityDepositService.CalculateDividendsAsync(year);

                        if (dividends.Any())
                        {
                            _logger.LogInformation(
                                "Calculated {Count} dividend(s) for organization {OrganizationId} for year {Year}. " +
                                "Total tenant share: ${TenantShare:N2}",
                                dividends.Count,
                                organizationId,
                                year,
                                dividends.Sum(d => d.DividendAmount));
                        }
                        else
                        {
                            _logger.LogInformation(
                                "No dividends to calculate for organization {OrganizationId} for year {Year}",
                                organizationId, year);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error processing dividends for organization {OrganizationId} for year {Year}",
                            organizationId, year);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing year-end dividends for year {Year}", year);
            }
        }

        /// <summary>
        /// Expires leases that have passed their end date using LeaseWorkflowService.
        /// This provides proper audit logging for lease expiration.
        /// </summary>
        private async Task<int> ExpireOverdueLeases(IServiceScope scope, Guid organizationId)
        {
            try
            {
                var leaseWorkflowService = scope.ServiceProvider.GetRequiredService<LeaseWorkflowService>();
                var result = await leaseWorkflowService.ExpireOverdueLeaseAsync(organizationId);

                if (result.Success)
                {
                    return result.Data;
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to expire overdue leases for organization {OrganizationId}: {Errors}",
                        organizationId,
                        string.Join(", ", result.Errors));
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring overdue leases for organization {OrganizationId}", organizationId);
                return 0;
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service is stopping.");
            _timer?.Dispose();
            _dailyTimer?.Change(Timeout.Infinite, 0);
            _hourlyTimer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            _dailyTimer?.Dispose();
            _hourlyTimer?.Dispose();
            base.Dispose();
        }
    }
}
