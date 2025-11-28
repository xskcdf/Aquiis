
ALTER TABLE Tenants ADD IdentificationNumber TEXT;
ALTER TABLE Tenants ADD IsActive INTEGER NOT NULL DEFAULT 1;

-- Update existing tenants to set IsActive to true
UPDATE Tenants
SET IsActive = 1
WHERE IsActive IS NULL;

-- Create index on IsActive for efficient querying
CREATE INDEX IF NOT EXISTS IX_Tenants_IsActive ON Tenants(IsActive);

UPDATE Tenants
SET IdentificationNumber = ''
WHERE IdentificationNumber IS NULL;


-- Migration: Create Inspections table
-- Date: November 15, 2025
-- Description: Creates table for tracking property inspections
