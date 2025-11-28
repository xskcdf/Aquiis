ALTER TABLE Tenants
ADD CreatedOn TEXT NOT NULL DEFAULT '';
    
    --LastModifiedBy nvarchar(100) NULL,
    --CreatedBy nvarchar(100) NOT NULL DEFAULT '',
    --IsDeleted bit NOT NULL DEFAULT 0;
GO

ALTER TABLE Tenants
ADD LastModifiedOn TEXT NULL;
GO

UPDATE Tenants
SET CreatedOn = CreatedDate,
    LastModifiedOn = LastModified;
    --CreatedBy = '',
    --LastModifiedBy = '';
GO

ALTER TABLE Tenants
DROP COLUMN CreatedDate;
GO

ALTER TABLE Tenants
DROP COLUMN LastModified;
GO
-- Repeat similar changes for other tables as needed
