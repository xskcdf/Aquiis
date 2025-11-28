-- Aquiis Database Creation Script
-- This script creates all necessary tables for the Aquiis property management system

-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Set journal mode to WAL for better performance
PRAGMA journal_mode = WAL;

-- Create Properties table
CREATE TABLE IF NOT EXISTS "Properties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Properties" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "City" TEXT NOT NULL,
    "State" TEXT NOT NULL,
    "ZipCode" TEXT NOT NULL,
    "PropertyType" TEXT NOT NULL,
    "MonthlyRent" DECIMAL(18,2) NOT NULL,
    "Bedrooms" INTEGER NOT NULL,
    "Bathrooms" DECIMAL(3,1) NOT NULL,
    "SquareFeet" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "IsAvailable" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "LastModified" TEXT NULL,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);