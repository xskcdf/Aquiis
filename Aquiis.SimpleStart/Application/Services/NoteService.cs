using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Application.Services
{
    public class NoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;

        public NoteService(ApplicationDbContext context, UserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        /// <summary>
        /// Add a note to an entity
        /// </summary>
        public async Task<Note> AddNoteAsync(string entityType, Guid entityId, string content)
        {
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();
            var userId = await _userContext.GetUserIdAsync();
            var userFullName = await _userContext.GetUserNameAsync();
            var userEmail = await _userContext.GetUserEmailAsync();

            if (!organizationId.HasValue || string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User context is not available.");
            }

            var note = new Note
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId!.Value,
                EntityType = entityType,
                EntityId = entityId,
                Content = content.Trim(),
                UserFullName = !string.IsNullOrWhiteSpace(userFullName) ? userFullName : userEmail,
                CreatedBy = !string.IsNullOrEmpty(userId) ? userId : string.Empty,
                CreatedOn = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Get all notes for an entity, ordered by newest first
        /// </summary>
        public async Task<List<Note>> GetNotesAsync(string entityType, Guid entityId)
        {
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();
            return await _context.Notes
                .Include(n => n.User)
                .Where(n => n.EntityType == entityType 
                    && n.EntityId == entityId 
                    && n.OrganizationId == organizationId
                    && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
        }

        /// <summary>
        /// Delete a note (soft delete)
        /// </summary>
        public async Task<bool> DeleteNoteAsync(Guid noteId)
        {
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == noteId 
                    && n.OrganizationId == organizationId
                    && !n.IsDeleted);

            if (note == null)
                return false;

            var userId = await _userContext.GetUserIdAsync();
            note.IsDeleted = true;
            note.LastModifiedBy = userId;
            note.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get note count for an entity
        /// </summary>
        public async Task<int> GetNoteCountAsync(string entityType, Guid entityId)
        {
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();
            return await _context.Notes
                .CountAsync(n => n.EntityType == entityType 
                    && n.EntityId == entityId 
                    && n.OrganizationId == organizationId
                    && !n.IsDeleted);
        }
    }
}
