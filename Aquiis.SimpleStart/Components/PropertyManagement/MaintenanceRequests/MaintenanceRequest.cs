using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Models;

namespace Aquiis.SimpleStart.Components.PropertyManagement.MaintenanceRequests
{
    public class MaintenanceRequest : BaseModel, ISchedulableEntity
    {
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        public int PropertyId { get; set; }

        public int? CalendarEventId { get; set; }

        public int? LeaseId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RequestType { get; set; } = string.Empty; // From ApplicationConstants.MaintenanceRequestTypes

        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = "Medium"; // From ApplicationConstants.MaintenanceRequestPriorities

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Submitted"; // From ApplicationConstants.MaintenanceRequestStatuses

        [StringLength(500)]
        public string RequestedBy { get; set; } = string.Empty; // Name of person requesting

        [StringLength(100)]
        public string RequestedByEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string RequestedByPhone { get; set; } = string.Empty;

        public DateTime RequestedOn { get; set; } = DateTime.Today;

        public DateTime? ScheduledOn { get; set; }

        public DateTime? CompletedOn { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualCost { get; set; }

        [StringLength(100)]
        public string AssignedTo { get; set; } = string.Empty; // Contractor or maintenance person

        [StringLength(2000)]
        public string ResolutionNotes { get; set; } = string.Empty;

        // Navigation properties
        public virtual Property? Property { get; set; }
        public virtual Lease? Lease { get; set; }

        // Computed property for days open
        [NotMapped]
        public int DaysOpen
        {
            get
            {
                if (CompletedOn.HasValue)
                    return (CompletedOn.Value.Date - RequestedOn.Date).Days;
                
                return (DateTime.Today - RequestedOn.Date).Days;
            }
        }

        [NotMapped]
        public bool IsOverdue
        {
            get
            {
                if (Status == "Completed" || Status == "Cancelled")
                    return false;

                if (!ScheduledOn.HasValue)
                    return false;

                return DateTime.Today > ScheduledOn.Value.Date;
            }
        }

        [NotMapped]
        public string PriorityBadgeClass
        {
            get
            {
                return Priority switch
                {
                    "Urgent" => "bg-danger",
                    "High" => "bg-warning",
                    "Medium" => "bg-info",
                    "Low" => "bg-secondary",
                    _ => "bg-secondary"
                };
            }
        }

        [NotMapped]
        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    "Submitted" => "bg-primary",
                    "In Progress" => "bg-warning",
                    "Completed" => "bg-success",
                    "Cancelled" => "bg-secondary",
                    _ => "bg-secondary"
                };
            }
        }

        // ISchedulableEntity implementation
        public string GetEventTitle() => $"{RequestType}: {Title}";
        
        public DateTime GetEventStart() => ScheduledOn ?? RequestedOn;
        
        public int GetEventDuration() => 120; // Default 2 hours for maintenance
        
        public string GetEventType() => CalendarEventTypes.Maintenance;
        
        public int? GetPropertyId() => PropertyId;
        
        public string GetEventDescription() => $"{Property?.Address ?? "Property"} - {Priority} Priority";
        
        public string GetEventStatus() => Status;
    }
}
