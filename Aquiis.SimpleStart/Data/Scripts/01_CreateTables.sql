-- Aquiis Database Creation Script
-- This script creates all necessary tables for the Aquiis property management system

-- Enable foreign key constraints
PRAGMA foreign_keys = ON;

-- Set journal mode to WAL for better performance
PRAGMA journal_mode = WAL;

-- Create AspNetRoles table
CREATE TABLE IF NOT EXISTS "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NOT NULL CONSTRAINT "UQ_AspNetRoles_Name" UNIQUE,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    CONSTRAINT "UQ_AspNetRoles_Name" UNIQUE ("Name")
);

-- Create AspNetUsers table with custom fields
CREATE TABLE IF NOT EXISTS "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "LastLoginDate" TEXT NULL,
    "PreviousLoginDate" TEXT NULL,
    "LoginCount" INTEGER NOT NULL DEFAULT 0,
    "LastLoginIP" TEXT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL DEFAULT 0,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL DEFAULT 0,
    "TwoFactorEnabled" INTEGER NOT NULL DEFAULT 0,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL DEFAULT 0,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0
);

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
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS "Tenants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tenants" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "PhoneNumber" TEXT NOT NULL,
    "DateOfBirth" TEXT NULL,
    "EmergencyContactName" TEXT NULL,
    "EmergencyContactPhone" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "LastModified" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS "Leases" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "TenantId" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "MonthlyRent" REAL NOT NULL,
    "SecurityDeposit" REAL,
    "Status" TEXT NOT NULL DEFAULT 'Active',
    "Terms" TEXT,
    "Notes" TEXT,
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "LastModified" TEXT,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY("PropertyId") REFERENCES "Properties"("Id"),
    FOREIGN KEY("TenantId") REFERENCES "Tenants"("Id")
);

-- Create AspNetRoleClaims table
CREATE TABLE IF NOT EXISTS "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

-- Create AspNetUserClaims table
CREATE TABLE IF NOT EXISTS "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Create AspNetUserLogins table
CREATE TABLE IF NOT EXISTS "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Create AspNetUserRoles table
CREATE TABLE IF NOT EXISTS "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Create AspNetUserTokens table
CREATE TABLE IF NOT EXISTS "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);