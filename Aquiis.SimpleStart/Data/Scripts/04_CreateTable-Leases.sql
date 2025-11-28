
CREATE TABLE IF NOT EXISTS Leases(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PropertyId INTEGER NOT NULL,
    TenantId INTEGER NOT NULL,
    StartDate TEXT NOT NULL,
    EndDate TEXT NOT NULL,
    MonthlyRent REAL NOT NULL,
    SecurityDeposit REAL,
    Status TEXT NOT NULL,
    Terms TEXT,
    Notes TEXT,
    CreatedOn TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    LastModifiedOn TEXT,
    LastModifiedBy TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(PropertyId) REFERENCES Properties(Id),
    FOREIGN KEY(TenantId) REFERENCES Tenants(Id)
);