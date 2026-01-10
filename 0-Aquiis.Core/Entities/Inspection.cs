using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.Core.Entities
{

    public class Inspection : BaseModel, ISchedulableEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        public Guid PropertyId { get; set; }

        public Guid? CalendarEventId { get; set; }

        public Guid? LeaseId { get; set; }

        [Required]
        public DateTime CompletedOn { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string InspectionType { get; set; } = "Routine"; // Routine, Move-In, Move-Out, Maintenance

        [StringLength(100)]
        public string? InspectedBy { get; set; } = string.Empty;

        // Exterior Checklist
        public bool ExteriorRoofGood { get; set; }
        public string? ExteriorRoofNotes { get; set; }

        public bool ExteriorGuttersGood { get; set; }
        public string? ExteriorGuttersNotes { get; set; }

        public bool ExteriorSidingGood { get; set; }
        public string? ExteriorSidingNotes { get; set; }

        public bool ExteriorWindowsGood { get; set; }
        public string? ExteriorWindowsNotes { get; set; }

        public bool ExteriorDoorsGood { get; set; }
        public string? ExteriorDoorsNotes { get; set; }

        public bool ExteriorFoundationGood { get; set; }
        public string? ExteriorFoundationNotes { get; set; }

        public bool LandscapingGood { get; set; }
        public string? LandscapingNotes { get; set; }

        // Interior Checklist
        public bool InteriorWallsGood { get; set; }
        public string? InteriorWallsNotes { get; set; }

        public bool InteriorCeilingsGood { get; set; }
        public string? InteriorCeilingsNotes { get; set; }

        public bool InteriorFloorsGood { get; set; }
        public string? InteriorFloorsNotes { get; set; }

        public bool InteriorDoorsGood { get; set; }
        public string? InteriorDoorsNotes { get; set; }

        public bool InteriorWindowsGood { get; set; }
        public string? InteriorWindowsNotes { get; set; }

        // Kitchen
        public bool KitchenAppliancesGood { get; set; }
        public string? KitchenAppliancesNotes { get; set; }

        public bool KitchenCabinetsGood { get; set; }
        public string? KitchenCabinetsNotes { get; set; }

        public bool KitchenCountersGood { get; set; }
        public string? KitchenCountersNotes { get; set; }

        public bool KitchenSinkPlumbingGood { get; set; }
        public string? KitchenSinkPlumbingNotes { get; set; }

        // Bathroom
        public bool BathroomToiletGood { get; set; }
        public string? BathroomToiletNotes { get; set; }

        public bool BathroomSinkGood { get; set; }
        public string? BathroomSinkNotes { get; set; }

        public bool BathroomTubShowerGood { get; set; }
        public string? BathroomTubShowerNotes { get; set; }

        public bool BathroomVentilationGood { get; set; }
        public string? BathroomVentilationNotes { get; set; }

        // Systems
        public bool HvacSystemGood { get; set; }
        public string? HvacSystemNotes { get; set; }

        public bool ElectricalSystemGood { get; set; }
        public string? ElectricalSystemNotes { get; set; }

        public bool PlumbingSystemGood { get; set; }
        public string? PlumbingSystemNotes { get; set; }

        public bool SmokeDetectorsGood { get; set; }
        public string? SmokeDetectorsNotes { get; set; }

        public bool CarbonMonoxideDetectorsGood { get; set; }
        public string? CarbonMonoxideDetectorsNotes { get; set; }

        // Overall Assessment
        [Required]
        [StringLength(20)]
        public string OverallCondition { get; set; } = "Good"; // Excellent, Good, Fair, Poor

        [StringLength(2000)]
        public string? GeneralNotes { get; set; }

        [StringLength(2000)]
        public string? ActionItemsRequired { get; set; }

        // Generated PDF Document
        public Guid? DocumentId { get; set; }

        // Navigation Properties
        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }

        [ForeignKey("LeaseId")]
        public Lease? Lease { get; set; }

        [ForeignKey("DocumentId")]
        public Document? Document { get; set; }

        // Audit Fields
        // SEE BASE MODEL

        // ISchedulableEntity implementation
        public string GetEventTitle() => $"{InspectionType} Inspection: {Property?.Address ?? "Property"}";
        
        public DateTime GetEventStart() => CompletedOn;
        
        public int GetEventDuration() => 60; // Default 1 hour for inspections
        
        public string GetEventType() => CalendarEventTypes.Inspection;
        
        public Guid? GetPropertyId() => PropertyId;
        
        public string GetEventDescription() => $"{InspectionType} - {OverallCondition}";
        
        public string GetEventStatus() => OverallCondition;
    }
}
