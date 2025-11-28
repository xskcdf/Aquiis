DROP TABLE IF EXISTS Inspections;

-- Add Inspections table
CREATE TABLE IF NOT EXISTS Inspections (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    PropertyId INTEGER NOT NULL,
    LeaseId INTEGER,
    InspectionDate TEXT NOT NULL,
    InspectionType TEXT NOT NULL,
    InspectedBy TEXT,
    
    -- Exterior Checklist
    ExteriorRoofGood INTEGER NOT NULL DEFAULT 0,
    ExteriorRoofNotes TEXT,
    ExteriorGuttersGood INTEGER NOT NULL DEFAULT 0,
    ExteriorGuttersNotes TEXT,
    ExteriorSidingGood INTEGER NOT NULL DEFAULT 0,
    ExteriorSidingNotes TEXT,
    ExteriorWindowsGood INTEGER NOT NULL DEFAULT 0,
    ExteriorWindowsNotes TEXT,
    ExteriorDoorsGood INTEGER NOT NULL DEFAULT 0,
    ExteriorDoorsNotes TEXT,
    ExteriorFoundationGood INTEGER NOT NULL DEFAULT 0,
    ExteriorFoundationNotes TEXT,
    LandscapingGood INTEGER NOT NULL DEFAULT 0,
    LandscapingNotes TEXT,
    
    -- Interior Checklist
    InteriorWallsGood INTEGER NOT NULL DEFAULT 0,
    InteriorWallsNotes TEXT,
    InteriorCeilingsGood INTEGER NOT NULL DEFAULT 0,
    InteriorCeilingsNotes TEXT,
    InteriorFloorsGood INTEGER NOT NULL DEFAULT 0,
    InteriorFloorsNotes TEXT,
    InteriorDoorsGood INTEGER NOT NULL DEFAULT 0,
    InteriorDoorsNotes TEXT,
    InteriorWindowsGood INTEGER NOT NULL DEFAULT 0,
    InteriorWindowsNotes TEXT,
    
    -- Kitchen
    KitchenAppliancesGood INTEGER NOT NULL DEFAULT 0,
    KitchenAppliancesNotes TEXT,
    KitchenCabinetsGood INTEGER NOT NULL DEFAULT 0,
    KitchenCabinetsNotes TEXT,
    KitchenCountersGood INTEGER NOT NULL DEFAULT 0,
    KitchenCountersNotes TEXT,
    KitchenSinkPlumbingGood INTEGER NOT NULL DEFAULT 0,
    KitchenSinkPlumbingNotes TEXT,
    
    -- Bathroom
    BathroomToiletGood INTEGER NOT NULL DEFAULT 0,
    BathroomToiletNotes TEXT,
    BathroomSinkGood INTEGER NOT NULL DEFAULT 0,
    BathroomSinkNotes TEXT,
    BathroomTubShowerGood INTEGER NOT NULL DEFAULT 0,
    BathroomTubShowerNotes TEXT,
    BathroomVentilationGood INTEGER NOT NULL DEFAULT 0,
    BathroomVentilationNotes TEXT,
    
    -- Systems
    HvacSystemGood INTEGER NOT NULL DEFAULT 0,
    HvacSystemNotes TEXT,
    ElectricalSystemGood INTEGER NOT NULL DEFAULT 0,
    ElectricalSystemNotes TEXT,
    PlumbingSystemGood INTEGER NOT NULL DEFAULT 0,
    PlumbingSystemNotes TEXT,
    SmokeDetectorsGood INTEGER NOT NULL DEFAULT 0,
    SmokeDetectorsNotes TEXT,
    CarbonMonoxideDetectorsGood INTEGER NOT NULL DEFAULT 0,
    CarbonMonoxideDetectorsNotes TEXT,
    
    -- Overall Assessment
    OverallCondition TEXT NOT NULL DEFAULT 'Good',
    GeneralNotes TEXT,
    ActionItemsRequired TEXT,
    
    -- Audit Fields
    CreatedOn TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    LastModifiedOn TEXT,
    LastModifiedBy TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    
    FOREIGN KEY (PropertyId) REFERENCES Properties(Id),
    FOREIGN KEY (LeaseId) REFERENCES Leases(Id)
);

-- Create indexes for common queries
CREATE INDEX IF NOT EXISTS IX_Inspections_PropertyId ON Inspections(PropertyId);
CREATE INDEX IF NOT EXISTS IX_Inspections_LeaseId ON Inspections(LeaseId);
CREATE INDEX IF NOT EXISTS IX_Inspections_OrganizationId ON Inspections(OrganizationId);
CREATE INDEX IF NOT EXISTS IX_Inspections_UserId ON Inspections(UserId);
CREATE INDEX IF NOT EXISTS IX_Inspections_InspectionDate ON Inspections(InspectionDate);
