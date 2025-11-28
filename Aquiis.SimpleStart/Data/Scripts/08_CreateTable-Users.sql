DROP TABLE IF EXISTS "AspNetUsers";

GO

CREATE TABLE IF NOT EXISTS "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "OrganizationId" INTEGER NOT NULL DEFAULT 0,
    "FirstName" TEXT NULL,
    "LastName" TEXT NULL,
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
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY ("OrganizationId") REFERENCES "Organizations"("Id"),
    CONSTRAINT "UQ_AspNetUsers_UserName" UNIQUE ("UserName"),
    CONSTRAINT "UQ_AspNetUsers_Email" UNIQUE ("Email")
);

GO