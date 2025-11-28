-- Aquiis Database Creation Script
-- This script creates all necessary tables for the Aquiis property management system

-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Set journal mode to WAL for better performance
PRAGMA journal_mode = WAL;

-- Drop tables if they exist
DROP TABLE IF EXISTS "Payments";
DROP TABLE IF EXISTS "Invoices";
DROP TABLE IF EXISTS "Leases";
DROP TABLE IF EXISTS "Tenants";
DROP TABLE IF EXISTS "Properties";
DROP TABLE IF EXISTS "Documents";