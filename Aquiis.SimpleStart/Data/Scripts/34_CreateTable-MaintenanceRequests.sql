-- Migration: Create MaintenanceRequests table
-- Date: November 14, 2025
-- Description: Creates table for tracking property maintenance requests

CREATE TABLE IF NOT EXISTS MaintenanceRequests (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId TEXT NOT NULL,
    PropertyId INTEGER NOT NULL,
    LeaseId INTEGER,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL,
    RequestType TEXT NOT NULL,
    Priority TEXT NOT NULL DEFAULT 'Medium',
    Status TEXT NOT NULL DEFAULT 'Submitted',
    RequestedBy TEXT,
    RequestedByEmail TEXT,
    RequestedByPhone TEXT,
    RequestedOn TEXT NOT NULL,
    ScheduledOn TEXT,
    CompletedOn TEXT,
    EstimatedCost REAL DEFAULT 0,
    ActualCost REAL DEFAULT 0,
    AssignedTo TEXT,
    ResolutionNotes TEXT,
    CreatedOn TEXT NOT NULL,
    CreatedBy TEXT,
    LastModifiedOn TEXT,
    LastModifiedBy TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (PropertyId) REFERENCES Properties(Id),
    FOREIGN KEY (LeaseId) REFERENCES Leases(Id)
);

-- Create indexes for efficient querying
CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_OrganizationId 
ON MaintenanceRequests(OrganizationId);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_PropertyId 
ON MaintenanceRequests(PropertyId);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_LeaseId 
ON MaintenanceRequests(LeaseId);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_Status 
ON MaintenanceRequests(Status);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_Priority 
ON MaintenanceRequests(Priority);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_RequestedOn 
ON MaintenanceRequests(RequestedOn);

CREATE INDEX IF NOT EXISTS IX_MaintenanceRequests_ScheduledOn 
ON MaintenanceRequests(ScheduledOn);