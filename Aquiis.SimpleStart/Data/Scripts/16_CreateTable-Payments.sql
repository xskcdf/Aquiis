CREATE TABLE IF NOT EXISTS "Payments" (
    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "LeaseId" INTEGER NOT NULL DEFAULT 0,
    "InvoiceId" INTEGER NOT NULL DEFAULT 0,
    "UserId" TEXT NOT NULL,
    "InvoiceNumber" TEXT NOT NULL DEFAULT '',
    "Description" TEXT,
    "PaymentDate" TEXT,
    "Amount" REAL,
    "PaymentMethod" TEXT, --Cash, Check, CreditCard, BankTransfer
    "ReferenceNumber" TEXT,
    "PaymentStatus" TEXT NOT NULL DEFAULT 'Completed', --Completed, Pending, Failed
    "Notes" TEXT DEFAULT '',
    "Status" TEXT NOT NULL DEFAULT 'Pending', --Pending, Paid, Overdue, Cancelled
    "CreatedDate" TEXT NOT NULL DEFAULT (datetime('now')),
    "CreatedBy" TEXT NOT NULL DEFAULT '',
    "LastModified" TEXT,
    "LastModifiedBy" TEXT NOT NULL DEFAULT '',
    "IsDeleted" INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY("InvoiceId") REFERENCES "Invoices"("Id")
);