
CREATE TABLE Organizations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Name TEXT NULL,
    Address TEXT NULL,
    City TEXT NULL,
    State TEXT NULL,
    PostalCode TEXT NULL,
    PhoneNumber TEXT NULL,
    Email TEXT NULL,
    CreatedBy TEXT NOT NULL,
    CreatedOn TEXT NOT NULL,
    LastModifiedBy TEXT NULL,
    LastModifiedOn TEXT NULL,
    IsDeleted INTEGER NOT NULL DEFAULT 0
);

GO
CREATE INDEX IDX_Organizations_UserId ON Organizations (UserId);
GO