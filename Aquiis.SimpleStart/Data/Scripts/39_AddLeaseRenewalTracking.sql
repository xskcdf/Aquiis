-- Add Lease Renewal Tracking columns to Leases table
-- Migration: 39_AddLeaseRenewalTracking
-- Date: November 16, 2025

ALTER TABLE Leases ADD RenewalNotificationSent INTEGER NULL;

ALTER TABLE Leases ADD RenewalNotificationSentOn TEXT NULL;

ALTER TABLE Leases ADD COLUMN RenewalReminderSentOn TEXT NULL;

ALTER TABLE Leases ADD COLUMN RenewalStatus TEXT NULL;

ALTER TABLE Leases ADD COLUMN RenewalOfferedOn TEXT NULL;

ALTER TABLE Leases ADD COLUMN RenewalResponseOn TEXT NULL;

ALTER TABLE Leases ADD COLUMN ProposedRenewalRent REAL NULL;

ALTER TABLE Leases ADD COLUMN RenewalNotes TEXT NULL;
