using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Models;
using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Checklists;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components.Administration.Application;

namespace Aquiis.SimpleStart.Services
{
    public class RentalApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChecklistService _checklistService;

        public RentalApplicationService(ApplicationDbContext context, ChecklistService checklistService)
        {
            _context = context;
            _checklistService = checklistService;
        }

        #region ProspectiveTenant CRUD

        public async Task<List<ProspectiveTenant>> GetAllProspectiveTenantsAsync(string organizationId)
        {
            return await _context.ProspectiveTenants
                .Where(pt => pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .Include(pt => pt.Tours)
                .Include(pt => pt.Application)
                .OrderByDescending(pt => pt.CreatedOn)
                .ToListAsync();
        }

        public async Task<ProspectiveTenant?> GetProspectiveTenantByIdAsync(int id, string organizationId)
        {
            return await _context.ProspectiveTenants
                .Where(pt => pt.Id == id && pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .Include(pt => pt.Tours)
                .Include(pt => pt.Application)
                .FirstOrDefaultAsync();
        }

        public async Task<ProspectiveTenant> CreateProspectiveTenantAsync(ProspectiveTenant prospectiveTenant)
        {
            prospectiveTenant.CreatedOn = DateTime.UtcNow;
            prospectiveTenant.Status = ApplicationConstants.ProspectiveStatuses.Lead;
            prospectiveTenant.FirstContactedOn = DateTime.UtcNow;

            _context.ProspectiveTenants.Add(prospectiveTenant);
            await _context.SaveChangesAsync();
            return prospectiveTenant;
        }

        public async Task<ProspectiveTenant> UpdateProspectiveTenantAsync(ProspectiveTenant prospectiveTenant)
        {
            prospectiveTenant.LastModifiedOn = DateTime.UtcNow;
            _context.ProspectiveTenants.Update(prospectiveTenant);
            await _context.SaveChangesAsync();
            return prospectiveTenant;
        }

        public async Task DeleteProspectiveTenantAsync(int id, string organizationId, string deletedBy)
        {
            var prospectiveTenant = await GetProspectiveTenantByIdAsync(id, organizationId);
            if (prospectiveTenant != null)
            {
                prospectiveTenant.IsDeleted = true;
                prospectiveTenant.LastModifiedOn = DateTime.UtcNow;
                prospectiveTenant.LastModifiedBy = deletedBy;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Tour CRUD

        public async Task<List<Tour>> GetAllToursAsync(string organizationId)
        {
            return await _context.Tours
                .Where(s => s.OrganizationId == organizationId && !s.IsDeleted)
                .Include(s => s.ProspectiveTenant)
                .Include(s => s.Property)
                .Include(s => s.Checklist)
            .OrderBy(s => s.ScheduledOn)
            .ToListAsync();
    }

    public async Task<List<Tour>> GetToursByProspectiveIdAsync(int prospectiveTenantId, string organizationId)
    {
        return await _context.Tours
            .Where(s => s.ProspectiveTenantId == prospectiveTenantId && s.OrganizationId == organizationId && !s.IsDeleted)
            .Include(s => s.ProspectiveTenant)
            .Include(s => s.Property)
            .Include(s => s.Checklist)
            .OrderBy(s => s.ScheduledOn)
            .ToListAsync();
    }

    public async Task<Tour?> GetTourByIdAsync(int id, string organizationId)
    {
        return await _context.Tours
            .Where(s => s.Id == id && s.OrganizationId == organizationId && !s.IsDeleted)
            .Include(s => s.ProspectiveTenant)
            .Include(s => s.Property)
            .Include(s => s.Checklist)
            .FirstOrDefaultAsync();
    }

        public async Task<Tour> CreateTourAsync(Tour tour, int? templateId = null)
        {
            tour.CreatedOn = DateTime.UtcNow;
            tour.Status = ApplicationConstants.TourStatuses.Scheduled;

            // Get prospect information for checklist
            var prospective = await _context.ProspectiveTenants
                .Include(p => p.InterestedProperty)
                .FirstOrDefaultAsync(p => p.Id == tour.ProspectiveTenantId);

            // Find the specified template, or fall back to default "Property Tour" template
            ChecklistTemplate? tourTemplate = null;
            
            if (templateId.HasValue && templateId.Value > 0)
            {
                // Use the specified template
                tourTemplate = await _context.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId.Value && 
                        (t.OrganizationId == tour.OrganizationId || t.IsSystemTemplate) && 
                        !t.IsDeleted);
            }
            
            // Fall back to default "Property Tour" template if not specified or not found
            if (tourTemplate == null)
            {
                tourTemplate = await _context.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Name == "Property Tour" && 
                        (t.OrganizationId == tour.OrganizationId || t.IsSystemTemplate) && 
                        !t.IsDeleted);
            }

            if (tourTemplate != null && prospective != null)
            {
                // Create checklist from template
                var checklist = await _checklistService.CreateChecklistFromTemplateAsync(tourTemplate.Id);
                
                // Customize checklist with prospect information
                checklist.Name = $"Property Tour - {prospective.FullName}";
                checklist.PropertyId = tour.PropertyId;
                checklist.GeneralNotes = $"Prospect: {prospective.FullName}\n" +
                                        $"Email: {prospective.Email}\n" +
                                        $"Phone: {prospective.Phone}\n" +
                                    $"Scheduled: {tour.ScheduledOn:MMM dd, yyyy h:mm tt}";
                
                // Link tour to checklist
                tour.ChecklistId = checklist.Id;
            }

            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // Update ProspectiveTenant status
            if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.Lead)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.TourScheduled;
                prospective.LastModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return tour;
        }

        public async Task<Tour> UpdateTourAsync(Tour tour)
        {
            tour.LastModifiedOn = DateTime.UtcNow;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task DeleteTourAsync(int id, string organizationId, string deletedBy)
        {
            var tour = await GetTourByIdAsync(id, organizationId);
            if (tour != null)
            {
                tour.IsDeleted = true;
                tour.LastModifiedOn = DateTime.UtcNow;
                tour.LastModifiedBy = deletedBy;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CancelTourAsync(int tourId, string organizationId, string cancelledBy)
        {
            var tour = await GetTourByIdAsync(tourId, organizationId);
            if (tour == null) return false;

            // Update tour status to cancelled
            tour.Status = ApplicationConstants.TourStatuses.Cancelled;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = cancelledBy;
            await _context.SaveChangesAsync();

            // Check if prospect has any other scheduled tours
            var prospective = await _context.ProspectiveTenants.FindAsync(tour.ProspectiveTenantId);
            if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.TourScheduled)
            {
                var hasOtherScheduledTours = await _context.Tours
                    .AnyAsync(s => s.ProspectiveTenantId == tour.ProspectiveTenantId
                        && s.Id != tourId
                        && !s.IsDeleted
                        && s.Status == ApplicationConstants.TourStatuses.Scheduled);

                // If no other scheduled tours, revert prospect status to Lead
                if (!hasOtherScheduledTours)
                {
                    prospective.Status = ApplicationConstants.ProspectiveStatuses.Lead;
                    prospective.LastModifiedOn = DateTime.UtcNow;
                    prospective.LastModifiedBy = cancelledBy;
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> CompleteTourAsync(int tourId, string organizationId, string completedBy, string? feedback = null, string? interestLevel = null)
        {
            var tour = await GetTourByIdAsync(tourId, organizationId);
            if (tour == null) return false;

            // Update tour status and feedback
            tour.Status = ApplicationConstants.TourStatuses.Completed;
            tour.Feedback = feedback;
            tour.InterestLevel = interestLevel;
            tour.ConductedBy = completedBy;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = completedBy;
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion

        #region RentalApplication CRUD

        public async Task<List<RentalApplication>> GetAllRentalApplicationsAsync(string organizationId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .OrderByDescending(ra => ra.AppliedOn)
                .ToListAsync();
        }

        public async Task<RentalApplication?> GetRentalApplicationByIdAsync(int id, string organizationId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.Id == id && ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalApplication?> GetApplicationByProspectiveIdAsync(int prospectiveTenantId, string organizationId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.ProspectiveTenantId == prospectiveTenantId && ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalApplication> CreateRentalApplicationAsync(RentalApplication application)
        {
            application.CreatedOn = DateTime.UtcNow;
            application.AppliedOn = DateTime.UtcNow;
            application.Status = ApplicationConstants.ApplicationStatuses.Submitted;

            _context.RentalApplications.Add(application);
            await _context.SaveChangesAsync();

            // Update ProspectiveTenant status
            var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Applied;
                prospective.LastModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return application;
        }

        public async Task<RentalApplication> UpdateRentalApplicationAsync(RentalApplication application)
        {
            application.LastModifiedOn = DateTime.UtcNow;
            _context.RentalApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task DeleteRentalApplicationAsync(int id, string organizationId, string deletedBy)
        {
            var application = await GetRentalApplicationByIdAsync(id, organizationId);
            if (application != null)
            {
                application.IsDeleted = true;
                application.LastModifiedOn = DateTime.UtcNow;
                application.LastModifiedBy = deletedBy;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region ApplicationScreening CRUD

        public async Task<ApplicationScreening?> GetScreeningByApplicationIdAsync(int rentalApplicationId, string organizationId)
        {
            return await _context.ApplicationScreenings
                .Where(asc => asc.RentalApplicationId == rentalApplicationId && asc.OrganizationId == organizationId && !asc.IsDeleted)
                .Include(asc => asc.RentalApplication)
                .FirstOrDefaultAsync();
        }

        public async Task<ApplicationScreening> CreateScreeningAsync(ApplicationScreening screening)
        {
            screening.CreatedOn = DateTime.UtcNow;
            screening.OverallResult = ApplicationConstants.ScreeningResults.Pending;

            _context.ApplicationScreenings.Add(screening);
            await _context.SaveChangesAsync();

            // Update application and prospective tenant status
            var application = await _context.RentalApplications.FindAsync(screening.RentalApplicationId);
            if (application != null)
            {
                application.Status = ApplicationConstants.ApplicationStatuses.Screening;
                application.LastModifiedOn = DateTime.UtcNow;

                var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
                if (prospective != null)
                {
                    prospective.Status = ApplicationConstants.ProspectiveStatuses.Screening;
                    prospective.LastModifiedOn = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return screening;
        }

        public async Task<ApplicationScreening> UpdateScreeningAsync(ApplicationScreening screening)
        {
            screening.LastModifiedOn = DateTime.UtcNow;
            _context.ApplicationScreenings.Update(screening);
            await _context.SaveChangesAsync();
            return screening;
        }

        #endregion

        #region Business Logic

        public async Task<bool> ApproveApplicationAsync(int applicationId, string organizationId, string approvedBy)
        {
            var application = await GetRentalApplicationByIdAsync(applicationId, organizationId);
            if (application == null) return false;

            application.Status = ApplicationConstants.ApplicationStatuses.Approved;
            application.DecidedOn = DateTime.UtcNow;
            application.DecisionBy = approvedBy;
            application.LastModifiedOn = DateTime.UtcNow;
            application.LastModifiedBy = approvedBy;

            var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Approved;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = approvedBy;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DenyApplicationAsync(int applicationId, string organizationId, string deniedBy, string reason)
        {
            var application = await GetRentalApplicationByIdAsync(applicationId, organizationId);
            if (application == null) return false;

            application.Status = ApplicationConstants.ApplicationStatuses.Denied;
            application.DecidedOn = DateTime.UtcNow;
            application.DecisionBy = deniedBy;
            application.DenialReason = reason;
            application.LastModifiedOn = DateTime.UtcNow;
            application.LastModifiedBy = deniedBy;

            var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Denied;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = deniedBy;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProspectiveTenant>> GetProspectivesByStatusAsync(string status, string organizationId)
        {
            return await _context.ProspectiveTenants
                .Where(pt => pt.Status == status && pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .OrderByDescending(pt => pt.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetUpcomingToursAsync(string organizationId, int days = 7)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            return await _context.Tours
                .Where(s => s.OrganizationId == organizationId 
                    && !s.IsDeleted 
                    && s.Status == ApplicationConstants.TourStatuses.Scheduled
                && s.ScheduledOn >= startDate 
                && s.ScheduledOn <= endDate)
            .Include(s => s.ProspectiveTenant)
            .Include(s => s.Property)
            .Include(s => s.Checklist)
            .OrderBy(s => s.ScheduledOn)
            .ToListAsync();
        }

        public async Task<List<RentalApplication>> GetPendingApplicationsAsync(string organizationId)
        {
            return await _context.RentalApplications
                .Where(ra => ra.OrganizationId == organizationId 
                    && !ra.IsDeleted 
                    && (ra.Status == ApplicationConstants.ApplicationStatuses.Submitted 
                        || ra.Status == ApplicationConstants.ApplicationStatuses.UnderReview
                        || ra.Status == ApplicationConstants.ApplicationStatuses.Screening))
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .OrderBy(ra => ra.AppliedOn)
                .ToListAsync();
        }

        #endregion
    }
}
