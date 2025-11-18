-- Create OrganizationSettings table for organization-specific configuration
-- SQLite version

CREATE TABLE IF NOT EXISTS OrganizationSettings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrganizationId TEXT NOT NULL,
    
    -- Late Fee Settings
    LateFeeEnabled INTEGER NOT NULL DEFAULT 1,
    LateFeeAutoApply INTEGER NOT NULL DEFAULT 1,
    LateFeeGracePeriodDays INTEGER NOT NULL DEFAULT 3,
    LateFeePercentage REAL NOT NULL DEFAULT 0.05,
    MaxLateFeeAmount REAL NOT NULL DEFAULT 50.00,
    
    -- Payment Reminder Settings
    PaymentReminderEnabled INTEGER NOT NULL DEFAULT 1,
    PaymentReminderDaysBefore INTEGER NOT NULL DEFAULT 3,
    
    -- Audit Fields (from BaseModel)
    CreatedOn TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy TEXT NOT NULL,
    LastModifiedOn TEXT NULL,
    LastModifiedBy TEXT NULL,
    IsDeleted INTEGER NOT NULL DEFAULT 0,
    
    -- Constraints
    CONSTRAINT UQ_OrganizationSettings_OrganizationId UNIQUE (OrganizationId)
);

-- Create index for faster lookups
CREATE INDEX IF NOT EXISTS IX_OrganizationSettings_OrganizationId ON OrganizationSettings(OrganizationId);

-- Insert default settings for existing users (using AspNetUsers as organization source)
INSERT OR IGNORE INTO OrganizationSettings (
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
    CURRENT_TIMESTAMP,
    'System',
    0   -- IsDeleted
FROM AspNetUsers
WHERE OrganizationId IS NOT NULL
AND OrganizationId NOT IN (SELECT OrganizationId FROM OrganizationSettings WHERE OrganizationSettings.OrganizationId IS NOT NULL);
