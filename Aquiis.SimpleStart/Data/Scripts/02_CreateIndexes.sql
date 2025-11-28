-- Aquiis Database Indexes Script
-- This script creates all necessary indexes for optimal performance

-- Create indexes for AspNetRoleClaims
CREATE INDEX IF NOT EXISTS "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

-- Create indexes for AspNetRoles
CREATE UNIQUE INDEX IF NOT EXISTS "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

-- Create indexes for AspNetUserClaims
CREATE INDEX IF NOT EXISTS "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

-- Create indexes for AspNetUserLogins
CREATE INDEX IF NOT EXISTS "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

-- Create indexes for AspNetUserRoles
CREATE INDEX IF NOT EXISTS "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

-- Create indexes for AspNetUsers
CREATE INDEX IF NOT EXISTS "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE UNIQUE INDEX IF NOT EXISTS "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

-- Create indexes for Properties table for better query performance
CREATE INDEX IF NOT EXISTS "IX_Properties_UserId" ON "Properties" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Properties_PropertyType" ON "Properties" ("PropertyType");
CREATE INDEX IF NOT EXISTS "IX_Properties_IsAvailable" ON "Properties" ("IsAvailable");
CREATE INDEX IF NOT EXISTS "IX_Properties_City_State" ON "Properties" ("City", "State");
CREATE INDEX IF NOT EXISTS "IX_Properties_MonthlyRent" ON "Properties" ("MonthlyRent");
CREATE INDEX IF NOT EXISTS "IX_Properties_CreatedDate" ON "Properties" ("CreatedDate");