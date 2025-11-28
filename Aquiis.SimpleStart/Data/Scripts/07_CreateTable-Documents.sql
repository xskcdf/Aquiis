
CREATE TABLE IF NOT EXISTS Documents (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId INTEGER NOT NULL DEFAULT 0,
    LeaseId INTEGER NOT NULL DEFAULT 0,
    FileName TEXT NOT NULL,
    FileExtension TEXT NOT NULL,
    FileData BLOB NOT NULL,
    FileSize INTEGER NOT NULL,
    FileType TEXT,
    FilePath TEXT,
    DocumentType TEXT,
    Description TEXT,
    UploadedBy TEXT NOT NULL,
    CreatedBy TEXT NOT NULL DEFAULT '',
    CreatedOn TEXT,
    LastModifiedBy TEXT NOT NULL DEFAULT '',
    LastModifiedOn TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (LeaseId) REFERENCES Leases(Id),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);