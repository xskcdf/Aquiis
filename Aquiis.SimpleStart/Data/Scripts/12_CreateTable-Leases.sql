
CREATE TABLE IF NOT EXISTS "Leases" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "PropertyId" INTEGER NOT NULL DEFAULT 0,
    "TenantId" INTEGER NOT NULL DEFAULT 0,
    "StartDate" TEXT NOT NULL DEFAULT (datetime('now')),
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