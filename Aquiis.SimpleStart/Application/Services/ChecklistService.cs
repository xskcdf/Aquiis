using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Shared.Services;
using Aquiis.SimpleStart.Core.Constants;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Application.Services
{
    public class ChecklistService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserContextService _userContext;

        public ChecklistService(
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            UserContextService userContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userContext = userContext;
        }

        #region ChecklistTemplates

        public async Task<List<ChecklistTemplate>> GetChecklistTemplatesAsync()
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.ChecklistTemplates
                .Include(ct => ct.Items.OrderBy(i => i.ItemOrder))
                .Where(ct => !ct.IsDeleted && (ct.OrganizationId == organizationId || ct.IsSystemTemplate))
                .OrderBy(ct => ct.Name)
                .ToListAsync();
        }

        public async Task<ChecklistTemplate?> GetChecklistTemplateByIdAsync(int templateId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.ChecklistTemplates
                .Include(ct => ct.Items.OrderBy(i => i.ItemOrder))
                .FirstOrDefaultAsync(ct => ct.Id == templateId && !ct.IsDeleted && 
                    (ct.OrganizationId == organizationId || ct.IsSystemTemplate));
        }

        public async Task<ChecklistTemplate> AddChecklistTemplateAsync(ChecklistTemplate template)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            // Check for duplicate template name within organization
            var existingTemplate = await _dbContext.ChecklistTemplates
                .FirstOrDefaultAsync(t => t.Name == template.Name && 
                                         t.OrganizationId == organizationId && 
                                         !t.IsDeleted);
            
            if (existingTemplate != null)
            {
                throw new InvalidOperationException($"A template named '{template.Name}' already exists.");
            }

            template.OrganizationId = organizationId;
            template.CreatedBy = userId;
            template.CreatedOn = DateTime.UtcNow;

            _dbContext.ChecklistTemplates.Add(template);
            await _dbContext.SaveChangesAsync();

            return template;
        }

        public async Task UpdateChecklistTemplateAsync(ChecklistTemplate template)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            template.LastModifiedBy = userId;
            template.LastModifiedOn = DateTime.UtcNow;

            _dbContext.ChecklistTemplates.Update(template);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChecklistTemplateAsync(int templateId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var template = await _dbContext.ChecklistTemplates.FindAsync(templateId);
            if (template != null && !template.IsSystemTemplate)
            {
                template.IsDeleted = true;
                template.LastModifiedBy = userId;
                template.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region ChecklistTemplateItems

        public async Task<ChecklistTemplateItem> AddChecklistTemplateItemAsync(ChecklistTemplateItem item)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            item.OrganizationId = organizationId;
            item.CreatedBy = userId;
            item.CreatedOn = DateTime.UtcNow;

            _dbContext.ChecklistTemplateItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return item;
        }

        public async Task UpdateChecklistTemplateItemAsync(ChecklistTemplateItem item)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            item.LastModifiedBy = userId;
            item.LastModifiedOn = DateTime.UtcNow;

            _dbContext.ChecklistTemplateItems.Update(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChecklistTemplateItemAsync(int itemId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var item = await _dbContext.ChecklistTemplateItems.FindAsync(itemId);
            if (item != null)
            {
                _dbContext.ChecklistTemplateItems.Remove(item);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Checklists

        public async Task<List<Checklist>> GetChecklistsAsync(bool includeArchived = false)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var query = _dbContext.Checklists
                .Include(c => c.Property)
                .Include(c => c.Lease)
                .Include(c => c.ChecklistTemplate)
                .Include(c => c.Items.OrderBy(i => i.ItemOrder))
                .Where(c => c.OrganizationId == organizationId);

            if (includeArchived)
            {
                // Show only archived (soft deleted) checklists
                query = query.Where(c => c.IsDeleted);
            }
            else
            {
                // Show only active (not archived) checklists
                query = query.Where(c => !c.IsDeleted);
            }

            return await query.OrderByDescending(c => c.CreatedOn).ToListAsync();
        }

        public async Task<List<Checklist>> GetChecklistsByPropertyIdAsync(int propertyId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Checklists
                .Include(c => c.Property)
                .Include(c => c.Lease)
                .Include(c => c.ChecklistTemplate)
                .Include(c => c.Items.OrderBy(i => i.ItemOrder))
                .Where(c => !c.IsDeleted && c.OrganizationId == organizationId && c.PropertyId == propertyId)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Checklist>> GetChecklistsByLeaseIdAsync(int leaseId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Checklists
                .Include(c => c.Property)
                .Include(c => c.Lease)
                .Include(c => c.ChecklistTemplate)
                .Include(c => c.Items.OrderBy(i => i.ItemOrder))
                .Where(c => !c.IsDeleted && c.OrganizationId == organizationId && c.LeaseId == leaseId)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<Checklist?> GetChecklistByIdAsync(int checklistId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Checklists
                .Include(c => c.Property)
                .Include(c => c.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(c => c.ChecklistTemplate)
                .Include(c => c.Items.OrderBy(i => i.ItemOrder))
                .Include(c => c.Document)
                .FirstOrDefaultAsync(c => c.Id == checklistId && !c.IsDeleted && c.OrganizationId == organizationId);
        }

        /// <summary>
        /// Creates a new checklist instance from a template, including all template items
        /// </summary>
        public async Task<Checklist> CreateChecklistFromTemplateAsync(int templateId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            // Get the template with items
            var template = await GetChecklistTemplateByIdAsync(templateId);
            if (template == null)
            {
                throw new InvalidOperationException("Template not found.");
            }

            // Create the checklist from template
            var checklist = new Checklist
            {
                Name = template.Name,
                ChecklistType = template.Category,
                ChecklistTemplateId = template.Id,
                Status = ApplicationConstants.ChecklistStatuses.Draft,
                OrganizationId = organizationId,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            _dbContext.Checklists.Add(checklist);
            await _dbContext.SaveChangesAsync();

            // Create checklist items from template items
            foreach (var templateItem in template.Items)
            {
                var checklistItem = new ChecklistItem
                {
                    ChecklistId = checklist.Id,
                    ItemText = templateItem.ItemText,
                    ItemOrder = templateItem.ItemOrder,
                    CategorySection = templateItem.CategorySection,
                    SectionOrder = templateItem.SectionOrder,
                    RequiresValue = templateItem.RequiresValue,
                    IsChecked = false,
                    OrganizationId = organizationId
                };
                _dbContext.ChecklistItems.Add(checklistItem);
            }

            await _dbContext.SaveChangesAsync();

            // Reload with items
            return await GetChecklistByIdAsync(checklist.Id) ?? checklist;
        }

        public async Task<Checklist> AddChecklistAsync(Checklist checklist)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            checklist.OrganizationId = organizationId;
            checklist.CreatedBy = userId;
            checklist.CreatedOn = DateTime.UtcNow;

            _dbContext.Checklists.Add(checklist);
            await _dbContext.SaveChangesAsync();

            return checklist;
        }

        public async Task UpdateChecklistAsync(Checklist checklist)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            checklist.LastModifiedBy = userId;
            checklist.LastModifiedOn = DateTime.UtcNow;

            _dbContext.Checklists.Update(checklist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChecklistAsync(int checklistId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var checklist = await _dbContext.Checklists
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == checklistId && c.OrganizationId == organizationId);
            
            if (checklist != null)
            {
                // Completed checklists cannot be deleted, only archived
                if (checklist.Status == "Completed")
                {
                    throw new InvalidOperationException("Completed checklists cannot be deleted. Please archive them instead.");
                }

                // Hard delete - remove items first, then checklist
                _dbContext.ChecklistItems.RemoveRange(checklist.Items);
                _dbContext.Checklists.Remove(checklist);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task ArchiveChecklistAsync(int checklistId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var checklist = await _dbContext.Checklists
                .FirstOrDefaultAsync(c => c.Id == checklistId && c.OrganizationId == organizationId);
            
            if (checklist != null)
            {
                checklist.IsDeleted = true;
                checklist.LastModifiedBy = userId;
                checklist.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UnarchiveChecklistAsync(int checklistId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var checklist = await _dbContext.Checklists
                .FirstOrDefaultAsync(c => c.Id == checklistId && c.OrganizationId == organizationId);
            
            if (checklist != null)
            {
                checklist.IsDeleted = false;
                checklist.LastModifiedBy = userId;
                checklist.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CompleteChecklistAsync(int checklistId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var checklist = await _dbContext.Checklists.FindAsync(checklistId);
            if (checklist != null)
            {
                checklist.Status = "Completed";
                checklist.CompletedBy = userId;
                checklist.CompletedOn = DateTime.UtcNow;
                checklist.LastModifiedBy = userId;
                checklist.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                // Check if this is a Property Tour checklist linked to a tour
                var tour = await _dbContext.Tours
                    .Include(s => s.ProspectiveTenant)
                    .FirstOrDefaultAsync(s => s.ChecklistId == checklistId && !s.IsDeleted);

                if (tour != null)
                {
                    // Mark tour as completed
                    tour.Status = ApplicationConstants.TourStatuses.Completed;
                    tour.ConductedBy = userId;
                    tour.LastModifiedBy = userId;
                    tour.LastModifiedOn = DateTime.UtcNow;

                    // Update calendar event status
                    if (tour.CalendarEventId.HasValue)
                    {
                        var calendarEvent = await _dbContext.CalendarEvents
                            .FirstOrDefaultAsync(e => e.Id == tour.CalendarEventId.Value);
                        if (calendarEvent != null)
                        {
                            calendarEvent.Status = ApplicationConstants.TourStatuses.Completed;
                            calendarEvent.LastModifiedBy = userId;
                            calendarEvent.LastModifiedOn = DateTime.UtcNow;
                        }
                    }

                    // Update prospect status back to Lead (tour completed, awaiting application)
                    if (tour.ProspectiveTenant != null && 
                        tour.ProspectiveTenant.Status == ApplicationConstants.ProspectiveStatuses.TourScheduled)
                    {
                        // Check if they have other scheduled tours
                        var hasOtherScheduledTours = await _dbContext.Tours
                            .AnyAsync(s => s.ProspectiveTenantId == tour.ProspectiveTenantId
                                && s.Id != tour.Id
                                && !s.IsDeleted
                                && s.Status == ApplicationConstants.TourStatuses.Scheduled);

                        // Only revert to Lead if no other scheduled tours
                        if (!hasOtherScheduledTours)
                        {
                            tour.ProspectiveTenant.Status = ApplicationConstants.ProspectiveStatuses.Lead;
                            tour.ProspectiveTenant.LastModifiedBy = userId;
                            tour.ProspectiveTenant.LastModifiedOn = DateTime.UtcNow;
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<ChecklistTemplate> SaveChecklistAsTemplateAsync(int checklistId, string templateName, string? templateDescription = null)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            // Check for duplicate template name
            var existingTemplate = await _dbContext.ChecklistTemplates
                .FirstOrDefaultAsync(t => t.Name == templateName && 
                                         t.OrganizationId == organizationId && 
                                         !t.IsDeleted);
            
            if (existingTemplate != null)
            {
                throw new InvalidOperationException($"A template named '{templateName}' already exists. Please choose a different name.");
            }

            // Get the checklist with its items
            var checklist = await _dbContext.Checklists
                .Include(c => c.Items.OrderBy(i => i.ItemOrder))
                .FirstOrDefaultAsync(c => c.Id == checklistId && c.OrganizationId == organizationId);

            if (checklist == null)
            {
                throw new InvalidOperationException("Checklist not found.");
            }

            // Create new template
            var template = new ChecklistTemplate
            {
                Name = templateName,
                Description = templateDescription ?? $"Template created from checklist: {checklist.Name}",
                Category = checklist.ChecklistType,
                IsSystemTemplate = false,
                OrganizationId = organizationId,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            _dbContext.ChecklistTemplates.Add(template);
            await _dbContext.SaveChangesAsync();

            // Copy items to template
            foreach (var item in checklist.Items)
            {
                var templateItem = new ChecklistTemplateItem
                {
                    ChecklistTemplateId = template.Id,
                    ItemText = item.ItemText,
                    ItemOrder = item.ItemOrder,
                    CategorySection = item.CategorySection,
                    SectionOrder = item.SectionOrder,
                    IsRequired = false, // User can customize this later
                    RequiresValue = item.RequiresValue,
                    AllowsNotes = true,
                    OrganizationId = organizationId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                _dbContext.ChecklistTemplateItems.Add(templateItem);
            }

            await _dbContext.SaveChangesAsync();

            return template;
        }

        #endregion

        #region ChecklistItems

        public async Task<ChecklistItem> AddChecklistItemAsync(ChecklistItem item)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            item.OrganizationId = organizationId;
            item.CreatedBy = userId;
            item.CreatedOn = DateTime.UtcNow;

            _dbContext.ChecklistItems.Add(item);
            await _dbContext.SaveChangesAsync();

            return item;
        }

        public async Task UpdateChecklistItemAsync(ChecklistItem item)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            item.LastModifiedBy = userId;
            item.LastModifiedOn = DateTime.UtcNow;

            _dbContext.ChecklistItems.Update(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChecklistItemAsync(int itemId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var item = await _dbContext.ChecklistItems.FindAsync(itemId);
            if (item != null)
            {
                item.IsDeleted = true;
                item.LastModifiedBy = userId;
                item.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
