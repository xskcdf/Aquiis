using Microsoft.EntityFrameworkCore;
public class SampleFeatureContext : DbContext
{
    // This class can be used to hold context-specific information
    public SampleFeatureContext(DbContextOptions<SampleFeatureContext> options)
        : base(options)
    {
    }

}