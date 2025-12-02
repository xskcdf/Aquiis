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
        public async Task<Note> AddNoteAsync(string entityType, int entityId, string content)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            var userId = await _userContext.GetUserIdAsync();
            var userFullName = await _userContext.GetUserNameAsync();

            if (string.IsNullOrEmpty(organizationId) || string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User context is not available.");
            }

            var note = new Note
            {
                OrganizationId = organizationId,
                EntityType = entityType,
                EntityId = entityId,
                Content = content.Trim(),
                UserFullName = !string.IsNullOrWhiteSpace(userFullName) ? userFullName : "Unknown User",
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return note;
        }

        /// <summary>
        /// Get all notes for an entity, ordered by newest first
        /// </summary>
        public async Task<List<Note>> GetNotesAsync(string entityType, int entityId, string organizationId)
        {
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
        public async Task<bool> DeleteNoteAsync(int noteId, string organizationId)
        {
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
        public async Task<int> GetNoteCountAsync(string entityType, int entityId, string organizationId)
        {
            return await _context.Notes
                .CountAsync(n => n.EntityType == entityType 
                    && n.EntityId == entityId 
                    && n.OrganizationId == organizationId
                    && !n.IsDeleted);
        }
    }
}
