-- Migration: Add inspection tracking columns to Properties table
-- Date: November 13, 2025
-- Description: Adds columns for tracking routine inspection schedules

-- Add inspection tracking columns
ALTER TABLE Properties ADD COLUMN LastRoutineInspectionDate TEXT;
ALTER TABLE Properties ADD COLUMN NextRoutineInspectionDueDate TEXT;
ALTER TABLE Properties ADD COLUMN RoutineInspectionIntervalMonths INTEGER DEFAULT 12;

-- Update existing properties to set default interval
UPDATE Properties 
SET RoutineInspectionIntervalMonths = 12 
WHERE RoutineInspectionIntervalMonths IS NULL;

-- Set initial inspection due date for existing properties (30 days from now)
UPDATE Properties 
SET NextRoutineInspectionDueDate = datetime(CreatedOn, '+30 days')
WHERE NextRoutineInspectionDueDate IS NULL AND IsDeleted = 0;

-- Create index on NextRoutineInspectionDueDate for efficient querying
CREATE INDEX IF NOT EXISTS IX_Properties_NextRoutineInspectionDueDate 
ON Properties(NextRoutineInspectionDueDate);
