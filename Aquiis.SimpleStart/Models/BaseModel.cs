using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aquiis.SimpleStart.Models
{
    public class BaseModel
    {
        [Key]
        [JsonInclude]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [JsonInclude]
        [DataType(DataType.DateTime)]
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [JsonInclude]
        [StringLength(100)]
        [DataType(DataType.Text)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonInclude]
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Modified On")]
        public DateTime? LastModifiedOn { get; set; }

        [JsonInclude]
        [StringLength(100)]
        [DataType(DataType.Text)]
        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [JsonInclude]
        [Display(Name = "Is Deleted?")]
        public bool IsDeleted { get; set; } = false;
    }
}