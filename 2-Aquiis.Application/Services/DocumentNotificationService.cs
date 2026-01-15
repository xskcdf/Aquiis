using Aquiis.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service responsible for checking document expirations and sending reminders.
/// Monitors lease documents, insurance certificates, and property-related documents for expiration.
/// </summary>
public class DocumentNotificationService
{
    private readonly ILogger<DocumentNotificationService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly NotificationService _notificationService;

    public DocumentNotificationService(
        ILogger<DocumentNotificationService> logger,
        ApplicationDbContext dbContext,
        NotificationService notificationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Checks for old documents (lease agreements, insurance certs) that may need renewal.
    /// Sends notifications to admin users for documents older than 180 days.
    /// </summary>
    public async Task CheckDocumentExpirationsAsync()
    {
        try
        {
            _logger.LogInformation("Starting document expiration check at {Time}", DateTime.Now);

            var now = DateTime.Now;
            var days180 = now.AddDays(-180);

            var organizations = await _dbContext.Organizations
                .Where(o => !o.IsDeleted)
                .Select(o => o.Id)
                .ToListAsync();

            int notificationsCreated = 0;

            foreach (var organizationId in organizations)
            {
                // Get admin users for this organization
                var adminUsers = await _dbContext.OrganizationUsers
                    .Where(uom => uom.OrganizationId == organizationId &&
                                 uom.Role == "Admin" &&
                                 !uom.IsDeleted)
                    .ToListAsync();

                if (!adminUsers.Any())
                {
                    _logger.LogWarning("No admin users found for organization {OrgId}", organizationId);
                    continue;
                }

                // Check for old lease documents (6+ months old)
                var oldLeaseDocuments = await _dbContext.Documents
                    .Where(d => d.OrganizationId == organizationId &&
                              !d.IsDeleted &&
                              d.LeaseId != null &&
                              d.CreatedOn < days180 &&
                              d.DocumentType == ApplicationConstants.DocumentTypes.LeaseAgreement)
                    .Include(d => d.Lease)
                        .ThenInclude(l => l!.Property)
                    .Take(5) // Limit to 5 per check to avoid spam
                    .ToListAsync();

                foreach (var doc in oldLeaseDocuments)
                {
                    if (doc.Lease?.Property != null)
                    {
                        var daysOld = (now - doc.CreatedOn).Days;
                        var title = "Document Review Needed";
                        var message = $"Lease document for {doc.Lease.Property.Address} was created {daysOld} days ago. Please review if renewal or update is needed.";
                        
                        foreach (var user in adminUsers)
                        {
                            await _notificationService.SendNotificationAsync(
                                recipientUserId: user.UserId,
                                title: title,
                                message: message,
                                type: "System",
                                category: "DocumentReview",
                                relatedEntityId: doc.Id,
                                relatedEntityType: "Document"
                            );
                            notificationsCreated++;
                        }
                    }
                }

                // Check for old property insurance documents (6+ months old)
                // Note: Since Insurance/Certificate are not in DocumentTypes, checking description/filename
                var oldPropertyInsurance = await _dbContext.Documents
                    .Where(d => d.OrganizationId == organizationId &&
                              !d.IsDeleted &&
                              d.PropertyId != null &&
                              d.CreatedOn < days180 &&
                              (d.Description.ToLower().Contains("insurance") ||
                               d.Description.ToLower().Contains("certificate") ||
                               d.FileName.ToLower().Contains("insurance") ||
                               d.FileName.ToLower().Contains("certificate")))
                    .Include(d => d.Property)
                    .Take(5)
                    .ToListAsync();

                foreach (var doc in oldPropertyInsurance)
                {
                    if (doc.Property != null)
                    {
                        var daysOld = (now - doc.CreatedOn).Days;
                        var title = "Document Review Needed";
                        var message = $"Insurance/Certificate document for {doc.Property.Address} is {daysOld} days old. Please verify current coverage.";
                        
                        foreach (var user in adminUsers)
                        {
                            await _notificationService.SendNotificationAsync(
                                recipientUserId: user.UserId,
                                title: title,
                                message: message,
                                type: "System",
                                category: "DocumentReview",
                                relatedEntityId: doc.Id,
                                relatedEntityType: "Document"
                            );
                            notificationsCreated++;
                        }
                    }
                }

                // Check for old tenant documents - 1 year old
                var days365 = now.AddDays(-365);
                var oldTenantDocuments = await _dbContext.Documents
                    .Where(d => d.OrganizationId == organizationId &&
                              !d.IsDeleted &&
                              d.TenantId != null &&
                              d.CreatedOn < days365)
                    .Include(d => d.Tenant)
                    .Take(5)
                    .ToListAsync();

                foreach (var doc in oldTenantDocuments)
                {
                    if (doc.Tenant != null)
                    {
                        var daysOld = (now - doc.CreatedOn).Days;
                        var title = "Document Review Needed";
                        var message = $"{doc.DocumentType} for tenant {doc.Tenant.FullName} is {daysOld} days old. Please verify if update is needed.";
                        
                        foreach (var user in adminUsers)
                        {
                            await _notificationService.SendNotificationAsync(
                                recipientUserId: user.UserId,
                                title: title,
                                message: message,
                                type: "System",
                                category: "DocumentReview",
                                relatedEntityId: doc.Id,
                                relatedEntityType: "Document"
                            );
                            notificationsCreated++;
                        }
                    }
                }
            }

            _logger.LogInformation(
                "Document expiration check complete: {NotificationCount} notifications created across {OrgCount} organizations",
                notificationsCreated,
                organizations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking document expirations");
        }
    }
}
