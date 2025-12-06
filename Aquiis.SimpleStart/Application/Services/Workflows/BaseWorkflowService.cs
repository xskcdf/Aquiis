using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    /// <summary>
    /// Abstract base class for all workflow services.
    /// Provides transaction support, audit logging, and validation infrastructure.
    /// </summary>
    public abstract class BaseWorkflowService
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserContextService _userContext;

        protected BaseWorkflowService(
            ApplicationDbContext context,
            UserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        /// <summary>
        /// Executes a workflow operation within a database transaction.
        /// Automatically commits on success or rolls back on failure.
        /// </summary>
        protected async Task<WorkflowResult<T>> ExecuteWorkflowAsync<T>(
            Func<Task<WorkflowResult<T>>> workflowOperation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await workflowOperation();

                if (result.Success)
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return WorkflowResult<T>.Fail($"Workflow operation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a workflow operation within a database transaction (non-generic version).
        /// </summary>
        protected async Task<WorkflowResult> ExecuteWorkflowAsync(
            Func<Task<WorkflowResult>> workflowOperation)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await workflowOperation();

                if (result.Success)
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return WorkflowResult.Fail($"Workflow operation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a workflow state transition to the audit log.
        /// </summary>
        protected async Task LogTransitionAsync(
            string entityType,
            int entityId,
            string? fromStatus,
            string toStatus,
            string action,
            string? reason = null,
            Dictionary<string, object>? metadata = null)
        {
            var userId = await _userContext.GetUserIdAsync() ?? string.Empty;
            var activeOrgId = await _userContext.GetActiveOrganizationIdAsync();
            
            var auditLog = new WorkflowAuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                Action = action,
                Reason = reason,
                PerformedBy = userId,
                PerformedOn = DateTime.UtcNow,
                OrganizationId = int.Parse(activeOrgId ?? "0"),
                Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.WorkflowAuditLogs.Add(auditLog);
            // Note: SaveChangesAsync is called by ExecuteWorkflowAsync
        }

        /// <summary>
        /// Gets the complete audit history for an entity.
        /// </summary>
        public async Task<List<WorkflowAuditLog>> GetAuditHistoryAsync(
            string entityType,
            int entityId)
        {
            var activeOrgId = await _userContext.GetActiveOrganizationIdAsync();
            
            return await _context.WorkflowAuditLogs
                .Where(w => w.EntityType == entityType && w.EntityId == entityId)
                .Where(w => w.OrganizationId == int.Parse(activeOrgId ?? "0"))
                .OrderBy(w => w.PerformedOn)
                .ToListAsync();
        }

        /// <summary>
        /// Validates that an entity belongs to the active organization.
        /// </summary>
        protected async Task<bool> ValidateOrganizationOwnershipAsync<TEntity>(
            IQueryable<TEntity> query,
            int entityId) where TEntity : class
        {
            var activeOrgId = await _userContext.GetActiveOrganizationIdAsync();
            
            // This assumes entities have OrganizationId property
            // Override in derived classes if different validation needed
            var entity = await query
                .Where(e => EF.Property<int>(e, "Id") == entityId)
                .Where(e => EF.Property<string>(e, "OrganizationId") == activeOrgId)
                .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
                .FirstOrDefaultAsync();

            return entity != null;
        }

        /// <summary>
        /// Gets the current user ID from the user context.
        /// </summary>
        protected async Task<string> GetCurrentUserIdAsync()
        {
            return await _userContext.GetUserIdAsync() ?? string.Empty;
        }

        /// <summary>
        /// Gets the active organization ID from the user context.
        /// </summary>
        protected async Task<string> GetActiveOrganizationIdAsync()
        {
            return await _userContext.GetActiveOrganizationIdAsync() ?? string.Empty;
        }
    }
}
