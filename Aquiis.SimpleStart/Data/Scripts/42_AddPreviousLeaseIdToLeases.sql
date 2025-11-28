-- Add PreviousLeaseId column to Leases table for tracking lease renewals
-- Migration: 42_AddPreviousLeaseIdToLeases
-- Date: November 16, 2025

ALTER TABLE Leases ADD COLUMN PreviousLeaseId INTEGER NULL;
