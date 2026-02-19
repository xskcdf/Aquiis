using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    public class Tour : BaseModel, ISchedulableEntity
    {
        [RequiredGuid]
        [Display(Name = "Prospective Tenant")]
        public Guid ProspectiveTenantId { get; set; }

        [RequiredGuid]
        [Display(Name = "Property")]
        public Guid PropertyId { get; set; }

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
        public string? ConductedBy { get; set; } = string.Empty; // UserId of property manager

        [Display(Name = "Property Tour Checklist")]
        public Guid? ChecklistId { get; set; } // Links to property tour checklist

        [Display(Name = "Calendar Event")]
        public Guid? CalendarEventId { get; set; }

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
        
        public Guid? GetPropertyId() => PropertyId;
        
        public string GetEventDescription() => Property?.Address ?? string.Empty;
        
        public string GetEventStatus() => Status;
    }
}
