-- Create OrganizationSettings table for organization-specific configuration
-- SQL Server version

IF OBJECT_ID('dbo.OrganizationSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrganizationSettings (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrganizationId NVARCHAR(450) NOT NULL,
        
        -- Late Fee Settings
        LateFeeEnabled BIT NOT NULL CONSTRAINT DF_OrganizationSettings_LateFeeEnabled DEFAULT(1),
        LateFeeAutoApply BIT NOT NULL CONSTRAINT DF_OrganizationSettings_LateFeeAutoApply DEFAULT(1),
        LateFeeGracePeriodDays INT NOT NULL CONSTRAINT DF_OrganizationSettings_LateFeeGracePeriodDays DEFAULT(3),
        LateFeePercentage DECIMAL(5,4) NOT NULL CONSTRAINT DF_OrganizationSettings_LateFeePercentage DEFAULT(0.05),
        MaxLateFeeAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_OrganizationSettings_MaxLateFeeAmount DEFAULT(50.00),
        
        -- Payment Reminder Settings
        PaymentReminderEnabled BIT NOT NULL CONSTRAINT DF_OrganizationSettings_PaymentReminderEnabled DEFAULT(1),
        PaymentReminderDaysBefore INT NOT NULL CONSTRAINT DF_OrganizationSettings_PaymentReminderDaysBefore DEFAULT(3),
        
        -- Audit Fields (from BaseModel)
        CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_OrganizationSettings_CreatedOn DEFAULT(GETDATE()),
        CreatedBy NVARCHAR(450) NOT NULL,
        LastModifiedOn DATETIME2 NULL,
        LastModifiedBy NVARCHAR(450) NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_OrganizationSettings_IsDeleted DEFAULT(0),
        
        -- Constraints
        CONSTRAINT UQ_OrganizationSettings_OrganizationId UNIQUE (OrganizationId)
    );
END

-- Create index for faster lookups (if not exists)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes i
    JOIN sys.objects o ON i.object_id = o.object_id
    WHERE i.name = 'IX_OrganizationSettings_OrganizationId' AND o.object_id = OBJECT_ID('dbo.OrganizationSettings')
)
BEGIN
    CREATE INDEX IX_OrganizationSettings_OrganizationId ON dbo.OrganizationSettings(OrganizationId);
END

-- Insert default settings for existing users (using AspNetUsers as organization source)
INSERT INTO OrganizationSettings (
    OrganizationId,
    LateFeeEnabled,
    LateFeeAutoApply,
    LateFeeGracePeriodDays,
    LateFeePercentage,
    MaxLateFeeAmount,
    PaymentReminderEnabled,
    PaymentReminderDaysBefore,
    CreatedOn,
    CreatedBy,
    IsDeleted
)
SELECT DISTINCT
    OrganizationId,
    1,  -- LateFeeEnabled
    1,  -- LateFeeAutoApply
    3,  -- LateFeeGracePeriodDays
    0.05, -- LateFeePercentage (5%)
    50.00, -- MaxLateFeeAmount
    1,  -- PaymentReminderEnabled
    3,  -- PaymentReminderDaysBefore
    GETDATE(),
    'System',
    0   -- IsDeleted
FROM AspNetUsers
WHERE OrganizationId IS NOT NULL
AND OrganizationId NOT IN (SELECT OrganizationId FROM OrganizationSettings WHERE OrganizationSettings.OrganizationId IS NOT NULL);
