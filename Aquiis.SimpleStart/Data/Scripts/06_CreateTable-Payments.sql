
CREATE TABLE IF NOT EXISTS Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    InvoiceId INTEGER NOT NULL,
    InvoiceNumber TEXT,
    Description TEXT,
    PaymentDate TEXT NOT NULL,
    Amount REAL NOT NULL,
    PaymentMethod TEXT, --Cash, Check, CreditCard, BankTransfer
    ReferenceNumber TEXT,
    PaymentStatus TEXT NOT NULL, --Completed, Pending, Failed
    Notes TEXT DEFAULT '',
    Status TEXT NOT NULL, --Pending, Paid, Overdue, Cancelled
    CreatedOn TEXT NOT NULL,
    CreatedBy TEXT NOT NULL,
    LastModifiedOn TEXT,
    LastModifiedBy TEXT,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id)
);