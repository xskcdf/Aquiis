
CREATE TABLE IF NOT EXISTS Invoices (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    LeaseId INTEGER NOT NULL,
    InvoiceNumber TEXT NOT NULL UNIQUE,
    InvoiceDate TEXT NOT NULL,
    DueDate TEXT NOT NULL,
    Amount REAL NOT NULL,
    Description TEXT,
    DatePaid TEXT,
    AmountPaid REAL,
    Status TEXT NOT NULL, --Pending, Paid, Overdue, Cancelled
    Notes TEXT,
    CreatedOn TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    LastModifiedOn TEXT,
    LastModifiedBy TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(LeaseId) REFERENCES Leases(Id)
);