using System.Linq.Expressions;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Interfaces;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Core.Services
{
    /// <summary>
    /// Abstract base service providing common CRUD operations for entities.
    /// Implements organization-based multi-tenancy, soft delete support, 
    /// and automatic audit field management.
    /// </summary>
    /// <typeparam name="TEntity">Entity type that inherits from BaseModel</typeparam>
    public abstract class BaseService<TEntity> where TEntity : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<BaseService<TEntity>> _logger;
        protected readonly UserContextService _userContext;
        protected readonly ApplicationSettings _settings;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseService(
            ApplicationDbContext context,
            ILogger<BaseService<TEntity>> logger,
            UserContextService userContext,
            IOptions<ApplicationSettings> settings)
        {
            _context = context;
            _logger = logger;
            _userContext = userContext;
            _settings = settings.Value;
            _dbSet = context.Set<TEntity>();
        }

        #region CRUD Operations

        /// <summary>
        /// Retrieves an entity by its ID with organization isolation.
        /// Returns null if entity not found or belongs to different organization.
        /// Automatically filters out soft-deleted entities.
        /// </summary>
        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                var entity = await _dbSet
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

                if (entity == null)
                {
                    _logger.LogWarning($"{typeof(TEntity).Name} not found: {id}");
                    return null;
                }

                // Verify organization access if entity has OrganizationId property
                if (HasOrganizationIdProperty(entity))
                {
                    var entityOrgId = GetOrganizationId(entity);
                    if (entityOrgId != organizationId)
                    {
                        _logger.LogWarning($"Unauthorized access to {typeof(TEntity).Name} {id} from organization {organizationId}");
                        return null;
                    }
                }

                return entity;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, $"GetById{typeof(TEntity).Name}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves all entities for the current organization.
        /// Automatically filters out soft-deleted entities and applies organization isolation.
        /// </summary>
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                IQueryable<TEntity> query = _dbSet.Where(e => !e.IsDeleted);

                // Apply organization filter if entity has OrganizationId property
                if (typeof(TEntity).GetProperty("OrganizationId") != null)
                {
                    var parameter = Expression.Parameter(typeof(TEntity), "e");
                    var property = Expression.Property(parameter, "OrganizationId");
                    var constant = Expression.Constant(organizationId);
                    var condition = Expression.Equal(property, constant);
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(condition, parameter);

                    query = query.Where(lambda);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, $"GetAll{typeof(TEntity).Name}");
                throw;
            }
        }

        /// <summary>
        /// Creates a new entity with automatic audit field and organization assignment.
        /// Validates entity before creation and sets CreatedBy, CreatedOn, and OrganizationId.
        /// </summary>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                // Validate entity
                await ValidateEntityAsync(entity);

                // Ensure ID is set
                if (entity.Id == Guid.Empty)
                {
                    entity.Id = Guid.NewGuid();
                }

                // Set audit fields
                SetAuditFieldsForCreate(entity, userId);

                // Set organization ID if property exists
                if (HasOrganizationIdProperty(entity) && organizationId.HasValue)
                {
                    SetOrganizationId(entity, organizationId.Value);
                }

                _dbSet.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"{typeof(TEntity).Name} created: {entity.Id} by user {userId}");

                return entity;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, $"Create{typeof(TEntity).Name}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing entity with automatic audit field management.
        /// Validates entity and organization ownership before update.
        /// Sets LastModifiedBy and LastModifiedOn automatically.
        /// </summary>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                // Validate entity
                await ValidateEntityAsync(entity);

                // Verify entity exists and belongs to organization
                var existing = await _dbSet
                    .FirstOrDefaultAsync(e => e.Id == entity.Id && !e.IsDeleted);

                if (existing == null)
                {
                    throw new InvalidOperationException($"{typeof(TEntity).Name} not found: {entity.Id}");
                }

                // Verify organization access
                if (HasOrganizationIdProperty(existing) && organizationId.HasValue)
                {
                    var existingOrgId = GetOrganizationId(existing);
                    if (existingOrgId != organizationId)
                    {
                        throw new UnauthorizedAccessException(
                            $"Cannot update {typeof(TEntity).Name} {entity.Id} - belongs to different organization.");
                    }

                    // Prevent organization hijacking
                    SetOrganizationId(entity, organizationId.Value);
                }

                // Set audit fields
                SetAuditFieldsForUpdate(entity, userId);

                // Update entity
                _context.Entry(existing).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"{typeof(TEntity).Name} updated: {entity.Id} by user {userId}");

                return entity;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, $"Update{typeof(TEntity).Name}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an entity (soft delete if enabled, hard delete otherwise).
        /// Verifies organization ownership before deletion.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                var entity = await _dbSet
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

                if (entity == null)
                {
                    _logger.LogWarning($"{typeof(TEntity).Name} not found for deletion: {id}");
                    return false;
                }

                // Verify organization access
                if (HasOrganizationIdProperty(entity) && organizationId.HasValue)
                {
                    var entityOrgId = GetOrganizationId(entity);
                    if (entityOrgId != organizationId)
                    {
                        throw new UnauthorizedAccessException(
                            $"Cannot delete {typeof(TEntity).Name} {id} - belongs to different organization.");
                    }
                }

                // Soft delete or hard delete based on settings
                if (_settings.SoftDeleteEnabled)
                {
                    entity.IsDeleted = true;
                    SetAuditFieldsForUpdate(entity, userId);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"{typeof(TEntity).Name} soft deleted: {id} by user {userId}");
                }
                else
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"{typeof(TEntity).Name} hard deleted: {id} by user {userId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, $"Delete{typeof(TEntity).Name}");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Virtual method for entity-specific validation.
        /// Override in derived classes to implement custom validation logic.
        /// </summary>
        protected virtual async Task ValidateEntityAsync(TEntity entity)
        {
            // Default: no validation
            // Override in derived classes for specific validation
            await Task.CompletedTask;
        }

        /// <summary>
        /// Virtual method for centralized exception handling.
        /// Override in derived classes for custom error handling logic.
        /// </summary>
        protected virtual async Task HandleExceptionAsync(Exception ex, string operation)
        {
            _logger.LogError(ex, $"Error in {operation} for {typeof(TEntity).Name}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// Sets audit fields when creating a new entity.
        /// </summary>
        protected virtual void SetAuditFieldsForCreate(TEntity entity, string userId)
        {
            entity.CreatedBy = userId;
            entity.CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets audit fields when updating an existing entity.
        /// </summary>
        protected virtual void SetAuditFieldsForUpdate(TEntity entity, string userId)
        {
            entity.LastModifiedBy = userId;
            entity.LastModifiedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if entity has OrganizationId property via reflection.
        /// </summary>
        private bool HasOrganizationIdProperty(TEntity entity)
        {
            return typeof(TEntity).GetProperty("OrganizationId") != null;
        }

        /// <summary>
        /// Gets the OrganizationId value from entity via reflection.
        /// </summary>
        private Guid? GetOrganizationId(TEntity entity)
        {
            var property = typeof(TEntity).GetProperty("OrganizationId");
            if (property == null) return null;

            var value = property.GetValue(entity);
            return value is Guid guidValue ? guidValue : null;
        }

        /// <summary>
        /// Sets the OrganizationId value on entity via reflection.
        /// </summary>
        private void SetOrganizationId(TEntity entity, Guid organizationId)
        {
            var property = typeof(TEntity).GetProperty("OrganizationId");
            property?.SetValue(entity, organizationId);
        }

        #endregion
    }
}
