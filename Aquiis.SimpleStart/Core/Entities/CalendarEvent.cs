using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Entities
{
    /// <summary>
    /// Represents a calendar event that can be either domain-linked (Tour, Inspection, etc.) 
    /// or a custom user-created event
    /// </summary>
    public class CalendarEvent : BaseModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start Date & Time")]
        public DateTime StartOn { get; set; }

        [Display(Name = "End Date & Time")]
        public DateTime? EndOn { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Event Type")]
        public string EventType { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Property")]
        public int? PropertyId { get; set; }

        [StringLength(500)]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [StringLength(20)]
        [Display(Name = "Color")]
        public string Color { get; set; } = "#6c757d"; // Default gray

        [StringLength(50)]
        [Display(Name = "Icon")]
        public string Icon { get; set; } = "bi-calendar-event";

        // Polymorphic reference to source entity (null for custom events)
        [Display(Name = "Source Entity ID")]
        public int? SourceEntityId { get; set; }

        [StringLength(100)]
        [Display(Name = "Source Entity Type")]
        public string? SourceEntityType { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        /// <summary>
        /// Indicates if this is a custom event (not linked to a domain entity)
        /// </summary>
        [NotMapped]
        public bool IsCustomEvent => string.IsNullOrEmpty(SourceEntityType);
    }
}
