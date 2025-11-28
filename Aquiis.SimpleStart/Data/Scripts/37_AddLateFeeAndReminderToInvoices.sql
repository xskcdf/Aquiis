-- Add Late Fee and Payment Reminder columns to Invoices table
-- SQLite version

ALTER TABLE Invoices ADD COLUMN LateFeeAmount REAL NULL;

ALTER TABLE Invoices ADD COLUMN LateFeeApplied INTEGER NULL;

ALTER TABLE Invoices ADD COLUMN LateFeeAppliedDate TEXT NULL;

ALTER TABLE Invoices ADD COLUMN ReminderSent INTEGER NULL;

ALTER TABLE Invoices ADD COLUMN ReminderSentDate TEXT NULL;
