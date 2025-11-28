-- Aquiis Database Creation Script
-- This script creates all necessary tables for the Aquiis property management system

-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Set journal mode to WAL for better performance
PRAGMA journal_mode = WAL;

CREATE TABLE IF NOT EXISTS "Tenants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tenants" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL UNIQUE,
    "PhoneNumber" TEXT NOT NULL,
    "DateOfBirth" TEXT NULL,
    "IdentificationNumber" TEXT NOT NULL UNIQUE,
    "EmergencyContactName" TEXT NULL,
    "EmergencyContactPhone" TEXT NULL,
    "Notes" TEXT NULL,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "LastModified" TEXT NULL,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);