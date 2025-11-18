using Aquiis.SimpleStart.Components.PropertyManagement;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquiis.SimpleStart.Services
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly ILogger<ScheduledTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;
        private Timer? _dailyTimer;
        private Timer? _hourlyTimer;

        public ScheduledTaskService(
            ILogger<ScheduledTaskService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                    var toastService = scope.ServiceProvider.GetRequiredService<ToastService>();
                    var propertyManagementService = scope.ServiceProvider.GetRequiredService<PropertyManagementService>();

                    // Get all distinct organization IDs from OrganizationSettings
                    var organizations = await dbContext.OrganizationSettings
                        .Where(s => !s.IsDeleted)
                        .Select(s => s.OrganizationId.ToString())
                        .Distinct()
                        .ToListAsync(stoppingToken);

                    foreach (var organizationId in organizations)
                    {
                        // Get settings for this organization
                        var settings = await propertyManagementService.GetOrganizationSettingsByOrgIdAsync(organizationId);
                        
                        if (settings == null)
                        {
                            _logger.LogWarning("No settings found for organization {OrganizationId}, skipping", organizationId);
                            continue;
                        }

                        // Task 1: Apply late fees to overdue invoices (if enabled)
                        if (settings.LateFeeEnabled && settings.LateFeeAutoApply)
                        {
                            await ApplyLateFees(dbContext, toastService, organizationId, settings, stoppingToken);
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
            ToastService toastService, 
            string organizationId,
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
                               i.UserId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueDate < today.AddDays(-settings.LateFeeGracePeriodDays) &&
                               (i.LateFeeApplied == null || !i.LateFeeApplied.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in overdueInvoices)
                {
                    var lateFee = Math.Min(invoice.Amount * settings.LateFeePercentage, settings.MaxLateFeeAmount);
                    
                    invoice.LateFeeAmount = lateFee;
                    invoice.LateFeeApplied = true;
                    invoice.LateFeeAppliedDate = DateTime.UtcNow;
                    invoice.Amount += lateFee;
                    invoice.Status = "Overdue";
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = "System";
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

        private async Task UpdateInvoiceStatuses(ApplicationDbContext dbContext, string organizationId, CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;

                // Update pending invoices that are now overdue (and haven't had late fees applied)
                var newlyOverdueInvoices = await dbContext.Invoices
                    .Where(i => !i.IsDeleted &&
                               i.UserId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueDate < today &&
                               (i.LateFeeApplied == null || !i.LateFeeApplied.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in newlyOverdueInvoices)
                {
                    invoice.Status = "Overdue";
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = "System";
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
            string organizationId,
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
                               i.UserId == organizationId &&
                               i.Status == "Pending" &&
                               i.DueDate >= today &&
                               i.DueDate <= today.AddDays(settings.PaymentReminderDaysBefore) &&
                               (i.ReminderSent == null || !i.ReminderSent.Value))
                    .ToListAsync(stoppingToken);

                foreach (var invoice in upcomingInvoices)
                {
                    // TODO: Integrate with email service when implemented
                    // For now, just log the reminder
                    _logger.LogInformation(
                        "Payment reminder needed for invoice {InvoiceNumber} due {DueDate} for tenant {TenantName} in organization {OrganizationId}",
                        invoice.InvoiceNumber,
                        invoice.DueDate.ToString("MMM dd, yyyy"),
                        invoice.Lease?.Tenant?.FullName ?? "Unknown",
                        organizationId);

                    invoice.ReminderSent = true;
                    invoice.ReminderSentDate = DateTime.UtcNow;
                    invoice.LastModifiedOn = DateTime.UtcNow;
                    invoice.LastModifiedBy = "System";
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

        private async Task CheckLeaseRenewals(ApplicationDbContext dbContext, string organizationId, CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Today;
                
                // Check for leases expiring in 90 days (initial notification)
                var leasesExpiring90Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.UserId == organizationId &&
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
                    lease.LastModifiedBy = "System";
                }

                // Check for leases expiring in 60 days (reminder)
                var leasesExpiring60Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.UserId == organizationId &&
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
                    lease.LastModifiedBy = "System";
                }

                // Check for leases expiring in 30 days (final reminder)
                var leasesExpiring30Days = await dbContext.Leases
                    .Include(l => l.Tenant)
                    .Include(l => l.Property)
                    .Where(l => !l.IsDeleted &&
                               l.UserId == organizationId &&
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
                }

                // Update status for expired leases
                var expiredLeases = await dbContext.Leases
                    .Where(l => !l.IsDeleted &&
                               l.UserId == organizationId &&
                               l.Status == "Active" &&
                               l.EndDate < today &&
                               (l.RenewalStatus == null || l.RenewalStatus == "Pending"))
                    .ToListAsync(stoppingToken);

                foreach (var lease in expiredLeases)
                {
                    lease.Status = "Expired";
                    lease.RenewalStatus = "Expired";
                    lease.LastModifiedOn = DateTime.UtcNow;
                    lease.LastModifiedBy = "System";

                    _logger.LogInformation(
                        "Lease expired: Lease ID {LeaseId}, End Date: {EndDate}",
                        lease.Id,
                        lease.EndDate.ToString("MMM dd, yyyy"));
                }

                var totalUpdated = leasesExpiring90Days.Count + leasesExpiring60Days.Count + 
                                  leasesExpiring30Days.Count + expiredLeases.Count;

                if (totalUpdated > 0)
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(
                        "Processed {Count} lease renewals for organization {OrganizationId}: {Initial} initial notifications, {Reminder60} 60-day reminders, {Reminder30} 30-day reminders, {Expired} expired",
                        totalUpdated,
                        organizationId,
                        leasesExpiring90Days.Count,
                        leasesExpiring60Days.Count,
                        leasesExpiring30Days.Count,
                        expiredLeases.Count);
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
                var propertyManagementService = scope.ServiceProvider.GetRequiredService<PropertyManagementService>();

                // Calculate daily payment totals
                var today = DateTime.Today;
                var todayPayments = await propertyManagementService.GetPaymentsAsync();
                var dailyTotal = todayPayments
                    .Where(p => p.PaymentDate.Date == today && !p.IsDeleted)
                    .Sum(p => p.Amount);

                _logger.LogInformation("Daily Payment Total for {Date}: ${Amount:N2}", 
                    today.ToString("yyyy-MM-dd"), 
                    dailyTotal);

                // Check for overdue routine inspections
                var overdueInspections = await propertyManagementService.GetPropertiesWithOverdueInspectionsAsync();
                if (overdueInspections.Any())
                {
                    _logger.LogWarning("{Count} propert(ies) have overdue routine inspections", 
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
                var dueSoonInspections = await propertyManagementService.GetPropertiesWithInspectionsDueSoonAsync(30);
                if (dueSoonInspections.Any())
                {
                    _logger.LogInformation("{Count} propert(ies) have routine inspections due within 30 days", 
                        dueSoonInspections.Count);
                }

                // You can add more daily tasks here:
                // - Generate daily reports
                // - Send payment reminders
                // - Check for overdue invoices
                // - Archive old records
                // - Send summary emails to property managers
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing daily tasks");
            }
        }

        private async Task ExecuteHourlyTasks()
        {
            _logger.LogInformation("Executing hourly tasks at {Time}", DateTime.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var propertyManagementService = scope.ServiceProvider.GetRequiredService<PropertyManagementService>();

                // Example hourly task: Check for upcoming lease expirations
                var upcomingLeases = await propertyManagementService.GetLeasesAsync();
                var expiringIn30Days = upcomingLeases
                    .Where(l => l.EndDate >= DateTime.Today && 
                               l.EndDate <= DateTime.Today.AddDays(30) && 
                               !l.IsDeleted)
                    .Count();

                if (expiringIn30Days > 0)
                {
                    _logger.LogInformation("{Count} lease(s) expiring in the next 30 days", expiringIn30Days);
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
