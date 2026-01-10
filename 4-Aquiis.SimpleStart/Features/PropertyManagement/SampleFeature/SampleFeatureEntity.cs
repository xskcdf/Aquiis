
public abstract class SampleFeatureEntity : BaseEntity, ISampleFeatureEntity
{
    public string Name { get; set; } = string.Empty;
}

public interface ISampleFeatureEntity
{
    Guid Id { get; set; }
    string Name { get; set; }

    
}

public abstract class BaseEntity
{
    // Base properties and methods for all entities can be defined here
    public Guid Id { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}