-- Aquiis Database Creation Script
-- This script creates all necessary tables for the Aquiis property management system

-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Set journal mode to WAL for better performance
PRAGMA journal_mode = WAL;

BEGIN TRANSACTION;

-- Truncate all tables
DELETE FROM Properties;
DELETE FROM Tenants;
--DELETE FROM Leases;
--DELETE FROM Invoices;
--DELETE FROM Payments;
--DELETE FROM Documents;

COMMIT;