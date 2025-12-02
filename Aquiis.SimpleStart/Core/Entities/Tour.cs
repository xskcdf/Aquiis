using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class Tour : BaseModel, ISchedulableEntity
    {
        [Required]
        [Display(Name = "Prospective Tenant")]
        public int ProspectiveTenantId { get; set; }

        [Required]
        [Display(Name = "Property")]
        public int PropertyId { get; set; }

        [Required]
        [Display(Name = "Scheduled Date & Time")]
        public DateTime ScheduledOn { get; set; }

        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty; // Scheduled, Completed, Cancelled, NoShow

        [StringLength(2000)]
        [Display(Name = "Feedback")]
        public string? Feedback { get; set; }

        [StringLength(50)]
        [Display(Name = "Interest Level")]
        public string? InterestLevel { get; set; } // VeryInterested, Interested, Neutral, NotInterested

        [StringLength(100)]
        [Display(Name = "Conducted By")]
        public string? ConductedBy { get; set; } // UserId of property manager

        [Display(Name = "Property Tour Checklist")]
        public int? ChecklistId { get; set; } // Links to property tour checklist

        [Display(Name = "Calendar Event")]
        public int? CalendarEventId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(ProspectiveTenantId))]
        public virtual ProspectiveTenant? ProspectiveTenant { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [ForeignKey(nameof(ChecklistId))]
        public virtual Checklist? Checklist { get; set; }

        // ISchedulableEntity implementation
        public string GetEventTitle() => $"Tour: {ProspectiveTenant?.FullName ?? "Prospect"}";
        
        public DateTime GetEventStart() => ScheduledOn;
        
        public int GetEventDuration() => DurationMinutes;
        
        public string GetEventType() => CalendarEventTypes.Tour;
        
        public int? GetPropertyId() => PropertyId;
        
        public string GetEventDescription() => Property?.Address ?? string.Empty;
        
        public string GetEventStatus() => Status;
    }
}
