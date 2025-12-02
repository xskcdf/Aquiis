using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Utilities;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Application.Services;

public class CalendarSettingsService
{
    private readonly ApplicationDbContext _context;
    private readonly UserContextService _userContext;

    public CalendarSettingsService(ApplicationDbContext context, UserContextService userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<List<CalendarSettings>> GetSettingsAsync(string organizationId)
    {
        await EnsureDefaultsAsync(organizationId);

        var orgId = Guid.Parse(organizationId);
        return await _context.CalendarSettings
            .Where(s => s.OrganizationId == orgId && !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.EntityType)
            .ToListAsync();
    }

    public async Task<CalendarSettings?> GetSettingAsync(string organizationId, string entityType)
    {
        var orgId = Guid.Parse(organizationId);
        var setting = await _context.CalendarSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == orgId 
                                   && s.EntityType == entityType 
                                   && !s.IsDeleted);

        if (setting == null)
        {
            // Create default if missing
            setting = CreateDefaultSetting(organizationId, entityType);
            _context.CalendarSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        return setting;
    }

    public async Task<CalendarSettings> UpdateSettingAsync(CalendarSettings setting)
    {
        var userId = await _userContext.GetUserIdAsync();
        setting.LastModifiedOn = DateTime.UtcNow;
        setting.LastModifiedBy = userId;

        _context.CalendarSettings.Update(setting);
        await _context.SaveChangesAsync();

        return setting;
    }

    public async Task<bool> IsAutoCreateEnabledAsync(string organizationId, string entityType)
    {
        var orgId = Guid.Parse(organizationId);
        var setting = await _context.CalendarSettings
            .FirstOrDefaultAsync(s => s.OrganizationId == orgId 
                                   && s.EntityType == entityType 
                                   && !s.IsDeleted);

        // Default to true if no setting exists
        return setting?.AutoCreateEvents ?? true;
    }

    public async Task EnsureDefaultsAsync(string organizationId)
    {
        var userId = await _userContext.GetUserIdAsync();
        var orgId = Guid.Parse(organizationId);
        var entityTypes = SchedulableEntityRegistry.GetEntityTypeNames();
        var existingSettings = await _context.CalendarSettings
            .Where(s => s.OrganizationId == orgId && !s.IsDeleted)
            .Select(s => s.EntityType)
            .ToListAsync();

        var missingTypes = entityTypes.Except(existingSettings).ToList();

        if (missingTypes.Any())
        {
            var newSettings = missingTypes.Select((entityType, index) =>
            {
                var setting = CreateDefaultSetting(organizationId, entityType);
                setting.DisplayOrder = existingSettings.Count + index;
                setting.CreatedBy = userId ?? string.Empty;
                setting.LastModifiedBy = userId;
                return setting;
            }).ToList();

            _context.CalendarSettings.AddRange(newSettings);
            await _context.SaveChangesAsync();
        }
    }

    private CalendarSettings CreateDefaultSetting(string organizationId, string entityType)
    {
        // Get defaults from CalendarEventTypes if available
        var config = CalendarEventTypes.Config.ContainsKey(entityType)
            ? CalendarEventTypes.Config[entityType]
            : null;

        return new CalendarSettings
        {
            OrganizationId = Guid.Parse(organizationId),
            EntityType = entityType,
            AutoCreateEvents = true,
            ShowOnCalendar = true,
            DefaultColor = config?.Color,
            DefaultIcon = config?.Icon,
            DisplayOrder = 0,
            CreatedOn = DateTime.UtcNow,
            LastModifiedOn = DateTime.UtcNow
        };
    }

    public async Task<List<CalendarSettings>> UpdateMultipleSettingsAsync(List<CalendarSettings> settings)
    {
        var userId = await _userContext.GetUserIdAsync();
        var now = DateTime.UtcNow;

        foreach (var setting in settings)
        {
            setting.LastModifiedOn = now;
            setting.LastModifiedBy = userId;
            _context.CalendarSettings.Update(setting);
        }

        await _context.SaveChangesAsync();
        return settings;
    }
}
