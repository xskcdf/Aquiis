-- Add late fee and reminder tracking columns to Invoices table
-- Migration: 37_AddLateFeeAndReminderToInvoices
-- Date: November 16, 2025

ALTER TABLE Invoices ADD COLUMN LateFeeAmount REAL NULL;
ALTER TABLE Invoices ADD COLUMN LateFeeApplied INTEGER NULL; -- SQLite uses INTEGER for boolean
ALTER TABLE Invoices ADD COLUMN LateFeeAppliedDate TEXT NULL; -- SQLite uses TEXT for DateTime
ALTER TABLE Invoices ADD COLUMN ReminderSent INTEGER NULL;
ALTER TABLE Invoices ADD COLUMN ReminderSentDate TEXT NULL;
