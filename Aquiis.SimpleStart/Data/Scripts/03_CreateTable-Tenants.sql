
CREATE TABLE IF NOT EXISTS Tenants(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId INTEGER NOT NULL,
    UserId TEXT NOT NULL UNIQUE, -- Login identifier
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE, -- Unique email address is also used for login
    PhoneNumber TEXT NULL,
    EmergencyContactName TEXT NULL,
    EmergencyContactPhone TEXT NULL,
    Notes TEXT NULL,
    CreatedBy TEXT NOT NULL,
    CreatedOn TEXT NOT NULL,
    LastModifiedBy TEXT NULL,
    LastModifiedOn TEXT NULL,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);