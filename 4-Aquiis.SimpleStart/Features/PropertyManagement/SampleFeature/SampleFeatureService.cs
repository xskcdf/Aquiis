
using Microsoft.EntityFrameworkCore;

public class SampleFeatureService
{
    private readonly SampleFeatureContext _context;

    public SampleFeatureService(SampleFeatureContext context)
    {
        _context = context;
    }

    public async Task<SampleFeatureEntity?> GetSampleFeatureByIdAsync(Guid id)
    {
        return await _context.Set<SampleFeatureEntity>()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // Sample methods for the service can be added here
}