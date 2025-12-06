using Aquiis.SimpleStart.Application.Services.Workflows;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    /// <summary>
    /// Application status enumeration for state machine validation.
    /// </summary>
    public enum ApplicationStatus
    {
        Submitted,
        UnderReview,
        Screening,
        Approved,
        Denied,
        LeaseOffered,
        LeaseAccepted,
        LeaseDeclined,
        Expired,
        Withdrawn
    }

    /// <summary>
    /// Workflow service for rental application lifecycle management.
    /// Centralizes all state transitions from prospect inquiry through lease offer generation.
    /// </summary>
    public class ApplicationWorkflowService : BaseWorkflowService, IWorkflowState<ApplicationStatus>
    {
        public ApplicationWorkflowService(
            ApplicationDbContext context,
            UserContextService userContext)
            : base(context, userContext)
        {
        }

        #region State Machine Implementation

        public bool IsValidTransition(ApplicationStatus fromStatus, ApplicationStatus toStatus)
        {
            var validTransitions = GetValidNextStates(fromStatus);
            return validTransitions.Contains(toStatus);
        }

        public List<ApplicationStatus> GetValidNextStates(ApplicationStatus currentStatus)
        {
            return currentStatus switch
            {
                ApplicationStatus.Submitted => new()
                {
                    ApplicationStatus.UnderReview,
                    ApplicationStatus.Denied,
                    ApplicationStatus.Withdrawn,
                    ApplicationStatus.Expired
                },
                ApplicationStatus.UnderReview => new()
                {
                    ApplicationStatus.Screening,
                    ApplicationStatus.Denied,
                    ApplicationStatus.Withdrawn,
                    ApplicationStatus.Expired
                },
                ApplicationStatus.Screening => new()
                {
                    ApplicationStatus.Approved,
                    ApplicationStatus.Denied,
                    ApplicationStatus.Withdrawn
                },
                ApplicationStatus.Approved => new()
                {
                    ApplicationStatus.LeaseOffered,
                    ApplicationStatus.Denied  // Can deny after approval if issues found
                },
                ApplicationStatus.LeaseOffered => new()
                {
                    ApplicationStatus.LeaseAccepted,
                    ApplicationStatus.LeaseDeclined,
                    ApplicationStatus.Expired
                },
                _ => new List<ApplicationStatus>() // Terminal states have no valid transitions
            };
        }

        public string GetInvalidTransitionReason(ApplicationStatus fromStatus, ApplicationStatus toStatus)
        {
            var validStates = GetValidNextStates(fromStatus);
            return $"Cannot transition from {fromStatus} to {toStatus}. Valid next states: {string.Join(", ", validStates)}";
        }

        #endregion

        #region Core Workflow Methods

        /// <summary>
        /// Submits a new rental application for a prospect and property.
        /// Creates application, updates property status if first app, and updates prospect status.
        /// </summary>
        public async Task<WorkflowResult<RentalApplication>> SubmitApplicationAsync(
            int prospectId,
            int propertyId,
            ApplicationSubmissionModel model)
        {
            return await ExecuteWorkflowAsync<RentalApplication>(async () =>
            {
                // Validation
                var validation = await ValidateApplicationSubmissionAsync(prospectId, propertyId);
                if (!validation.Success)
                    return WorkflowResult<RentalApplication>.Fail(validation.Errors);

                var userId = await GetCurrentUserIdAsync();
                var orgId = await GetActiveOrganizationIdAsync();

                // Get organization settings for expiration days
                var settings = await _context.OrganizationSettings
                    .FirstOrDefaultAsync(s => s.OrganizationId == orgId.ToString());
                
                var expirationDays = settings?.ApplicationExpirationDays ?? 30;

                // Create application
                var application = new RentalApplication
                {
                    OrganizationId = orgId.ToString(),
                    ProspectiveTenantId = prospectId,
                    PropertyId = propertyId,
                    Status = ApplicationConstants.ApplicationStatuses.Submitted,
                    AppliedOn = DateTime.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddDays(expirationDays),
                    ApplicationFee = model.ApplicationFee,
                    ApplicationFeePaid = model.ApplicationFeePaid,
                    ApplicationFeePaidOn = model.ApplicationFeePaid ? DateTime.UtcNow : null,
                    ApplicationFeePaymentMethod = model.ApplicationFeePaymentMethod,
                    CurrentAddress = model.CurrentAddress,
                    CurrentCity = model.CurrentCity,
                    CurrentState = model.CurrentState,
                    CurrentZipCode = model.CurrentZipCode,
                    CurrentRent = model.CurrentRent,
                    LandlordName = model.LandlordName,
                    LandlordPhone = model.LandlordPhone,
                    EmployerName = model.EmployerName,
                    JobTitle = model.JobTitle,
                    MonthlyIncome = model.MonthlyIncome,
                    EmploymentLengthMonths = model.EmploymentLengthMonths,
                    Reference1Name = model.Reference1Name,
                    Reference1Phone = model.Reference1Phone,
                    Reference1Relationship = model.Reference1Relationship,
                    Reference2Name = model.Reference2Name,
                    Reference2Phone = model.Reference2Phone,
                    Reference2Relationship = model.Reference2Relationship,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.RentalApplications.Add(application);
                await _context.SaveChangesAsync(); // Save to get application ID for logging

                // Update property status if this is first application
                var property = await _context.Properties
                    .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == orgId.ToString());
                
                if (property != null && property.Status == ApplicationConstants.PropertyStatuses.Available)
                {
                    property.Status = ApplicationConstants.PropertyStatuses.ApplicationPending;
                    property.LastModifiedBy = userId;
                    property.LastModifiedOn = DateTime.UtcNow;
                }

                // Update prospect status
                var prospect = await _context.ProspectiveTenants
                    .FirstOrDefaultAsync(p => p.Id == prospectId && p.OrganizationId == orgId.ToString());
                
                if (prospect != null)
                {
                    var oldStatus = prospect.Status;
                    prospect.Status = ApplicationConstants.ProspectiveStatuses.Applied;
                    prospect.LastModifiedBy = userId;
                    prospect.LastModifiedOn = DateTime.UtcNow;

                    // Log prospect transition
                    await LogTransitionAsync(
                        "ProspectiveTenant",
                        prospectId,
                        oldStatus,
                        prospect.Status,
                        "SubmitApplication");
                }

                // Log application creation
                await LogTransitionAsync(
                    "RentalApplication",
                    application.Id,
                    null,
                    ApplicationConstants.ApplicationStatuses.Submitted,
                    "SubmitApplication");

                return WorkflowResult<RentalApplication>.Ok(
                    application,
                    "Application submitted successfully");

            });
        }

        /// <summary>
        /// Marks an application as under manual review.
        /// </summary>
        public async Task<WorkflowResult> MarkApplicationUnderReviewAsync(int applicationId)
        {
            return await ExecuteWorkflowAsync(async () =>
            {
                var application = await GetApplicationAsync(applicationId);
                if (application == null)
                    return WorkflowResult.Fail("Application not found");

                // Validate state transition
                if (!IsValidTransition(
                    Enum.Parse<ApplicationStatus>(application.Status),
                    ApplicationStatus.UnderReview))
                {
                    return WorkflowResult.Fail(GetInvalidTransitionReason(
                        Enum.Parse<ApplicationStatus>(application.Status),
                        ApplicationStatus.UnderReview));
                }

                var userId = await GetCurrentUserIdAsync();
                var oldStatus = application.Status;

                application.Status = ApplicationConstants.ApplicationStatuses.UnderReview;
                application.DecisionBy = userId;
                application.LastModifiedBy = userId;
                application.LastModifiedOn = DateTime.UtcNow;

                await LogTransitionAsync(
                    "RentalApplication",
                    applicationId,
                    oldStatus,
                    application.Status,
                    "MarkUnderReview");

                return WorkflowResult.Ok("Application marked as under review");

            });
        }

        /// <summary>
        /// Initiates background and/or credit screening for an application.
        /// Requires application fee to be paid.
        /// </summary>
        public async Task<WorkflowResult<ApplicationScreening>> InitiateScreeningAsync(
            int applicationId,
            bool requestBackgroundCheck,
            bool requestCreditCheck)
        {
            return await ExecuteWorkflowAsync<ApplicationScreening>(async () =>
            {
                var application = await GetApplicationAsync(applicationId);
                if (application == null)
                    return WorkflowResult<ApplicationScreening>.Fail("Application not found");

                // Validate state
                if (application.Status != ApplicationConstants.ApplicationStatuses.UnderReview)
                    return WorkflowResult<ApplicationScreening>.Fail(
                        $"Application must be Under Review to initiate screening. Current status: {application.Status}");

                // Validate application fee paid
                if (!application.ApplicationFeePaid)
                    return WorkflowResult<ApplicationScreening>.Fail(
                        "Application fee must be paid before initiating screening");

                // Check for existing screening
                var existingScreening = await _context.ApplicationScreenings
                    .FirstOrDefaultAsync(s => s.RentalApplicationId == applicationId);
                
                if (existingScreening != null)
                    return WorkflowResult<ApplicationScreening>.Fail(
                        "Screening already exists for this application");

                var userId = await GetCurrentUserIdAsync();
                var orgId = await GetActiveOrganizationIdAsync();

                // Create screening record
                var screening = new ApplicationScreening
                {
                    OrganizationId = orgId.ToString(),
                    RentalApplicationId = applicationId,
                    BackgroundCheckRequested = requestBackgroundCheck,
                    BackgroundCheckRequestedOn = requestBackgroundCheck ? DateTime.UtcNow : null,
                    CreditCheckRequested = requestCreditCheck,
                    CreditCheckRequestedOn = requestCreditCheck ? DateTime.UtcNow : null,
                    OverallResult = "Pending",
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.ApplicationScreenings.Add(screening);

                // Update application status
                var oldStatus = application.Status;
                application.Status = ApplicationConstants.ApplicationStatuses.Screening;
                application.LastModifiedBy = userId;
                application.LastModifiedOn = DateTime.UtcNow;

                // Update prospect status
                if (application.ProspectiveTenant != null)
                {
                    application.ProspectiveTenant.Status = ApplicationConstants.ProspectiveStatuses.Screening;
                    application.ProspectiveTenant.LastModifiedBy = userId;
                    application.ProspectiveTenant.LastModifiedOn = DateTime.UtcNow;
                }

                await LogTransitionAsync(
                    "RentalApplication",
                    applicationId,
                    oldStatus,
                    application.Status,
                    "InitiateScreening");

                return WorkflowResult<ApplicationScreening>.Ok(
                    screening,
                    "Screening initiated successfully");

            });
        }

        #endregion

        #region Helper Methods

        private async Task<RentalApplication?> GetApplicationAsync(int applicationId)
        {
            var orgId = await GetActiveOrganizationIdAsync();
            return await _context.RentalApplications
                .Include(a => a.ProspectiveTenant)
                .Include(a => a.Property)
                .Include(a => a.Screening)
                .FirstOrDefaultAsync(a =>
                    a.Id == applicationId &&
                    a.OrganizationId == orgId.ToString() &&
                    !a.IsDeleted);
        }

        private async Task<WorkflowResult> ValidateApplicationSubmissionAsync(
            int prospectId,
            int propertyId)
        {
            var errors = new List<string>();
            var orgId = await GetActiveOrganizationIdAsync();

            // Validate prospect exists
            var prospect = await _context.ProspectiveTenants
                .FirstOrDefaultAsync(p => p.Id == prospectId && p.OrganizationId == orgId.ToString() && !p.IsDeleted);
            
            if (prospect == null)
                errors.Add("Prospect not found");
            else if (prospect.Status == ApplicationConstants.ProspectiveStatuses.ConvertedToTenant)
                errors.Add("Prospect has already been converted to a tenant");

            // Validate property exists and is available
            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == orgId.ToString() && !p.IsDeleted);
            
            if (property == null)
                errors.Add("Property not found");
            else if (property.Status == ApplicationConstants.PropertyStatuses.Occupied)
                errors.Add("Property is currently occupied");

            // Check for existing active application
            if (prospect != null)
            {
                var existingApp = await _context.RentalApplications
                    .AnyAsync(a =>
                        a.ProspectiveTenantId == prospectId &&
                        a.OrganizationId == orgId.ToString() &&
                        a.Status != ApplicationConstants.ApplicationStatuses.Denied &&
                        a.Status != ApplicationConstants.ApplicationStatuses.Withdrawn &&
                        a.Status != ApplicationConstants.ApplicationStatuses.Expired &&
                        a.Status != ApplicationConstants.ApplicationStatuses.LeaseDeclined &&
                        !a.IsDeleted);

                if (existingApp)
                    errors.Add("Prospect already has an active application");
            }

            return errors.Any()
                ? WorkflowResult.Fail(errors)
                : WorkflowResult.Ok();
        }

        #endregion
    }

    /// <summary>
    /// Model for application submission data.
    /// </summary>
    public class ApplicationSubmissionModel
    {
        public decimal ApplicationFee { get; set; }
        public bool ApplicationFeePaid { get; set; }
        public string? ApplicationFeePaymentMethod { get; set; }

        public string CurrentAddress { get; set; } = string.Empty;
        public string CurrentCity { get; set; } = string.Empty;
        public string CurrentState { get; set; } = string.Empty;
        public string CurrentZipCode { get; set; } = string.Empty;
        public decimal CurrentRent { get; set; }
        public string LandlordName { get; set; } = string.Empty;
        public string LandlordPhone { get; set; } = string.Empty;

        public string EmployerName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public decimal MonthlyIncome { get; set; }
        public int EmploymentLengthMonths { get; set; }

        public string Reference1Name { get; set; } = string.Empty;
        public string Reference1Phone { get; set; } = string.Empty;
        public string Reference1Relationship { get; set; } = string.Empty;
        public string? Reference2Name { get; set; }
        public string? Reference2Phone { get; set; }
        public string? Reference2Relationship { get; set; }
    }
}
