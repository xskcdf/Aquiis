
CREATE TABLE IF NOT EXISTS Properties (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId INTEGER NOT NULL,
    StreetAddress TEXT NOT NULL,
    City TEXT NOT NULL,
    State TEXT NOT NULL,
    PostalCode TEXT NOT NULL,
    PropertyType TEXT NOT NULL,
    MonthlyRent REAL NOT NULL,
    Bedrooms INTEGER NOT NULL,
    Bathrooms REAL NOT NULL,
    SquareFeet INTEGER NOT NULL,
    Description TEXT NULL,
    IsAvailable INTEGER NOT NULL DEFAULT 1,
    CreatedBy TEXT NOT NULL,
    CreatedOn TEXT NOT NULL,
    LastModifiedBy TEXT NULL,
    LastModifiedOn TEXT NULL,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);