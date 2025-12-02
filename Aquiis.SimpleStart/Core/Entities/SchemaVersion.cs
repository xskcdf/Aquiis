using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Core.Entities
{
    /// <summary>
    /// Tracks the database schema version for compatibility validation
    /// </summary>
    public class SchemaVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; } = string.Empty;

        public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
