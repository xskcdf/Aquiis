CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "OrganizationId" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "LastLoginDate" TEXT NULL,
    "PreviousLoginDate" TEXT NULL,
    "LoginCount" INTEGER NOT NULL,
    "LastLoginIP" TEXT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "CalendarSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CalendarSettings" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "EntityType" TEXT NOT NULL,
    "AutoCreateEvents" INTEGER NOT NULL,
    "ShowOnCalendar" INTEGER NOT NULL,
    "DefaultColor" TEXT NULL,
    "DefaultIcon" TEXT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL
);

CREATE TABLE "ChecklistTemplates" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ChecklistTemplates" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Category" TEXT NOT NULL,
    "IsSystemTemplate" INTEGER NOT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL
);

CREATE TABLE "OrganizationSettings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrganizationSettings" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "Name" TEXT NULL,
    "LateFeeEnabled" INTEGER NOT NULL,
    "LateFeeAutoApply" INTEGER NOT NULL,
    "LateFeeGracePeriodDays" INTEGER NOT NULL,
    "LateFeePercentage" TEXT NOT NULL,
    "MaxLateFeeAmount" TEXT NOT NULL,
    "PaymentReminderEnabled" INTEGER NOT NULL,
    "PaymentReminderDaysBefore" INTEGER NOT NULL,
    "TourNoShowGracePeriodHours" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL
);

CREATE TABLE "SchemaVersions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SchemaVersions" PRIMARY KEY AUTOINCREMENT,
    "Version" TEXT NOT NULL,
    "AppliedOn" TEXT NOT NULL,
    "Description" TEXT NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Notes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Notes" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "EntityType" TEXT NOT NULL,
    "EntityId" INTEGER NOT NULL,
    "UserFullName" TEXT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Notes_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Properties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Properties" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "UnitNumber" TEXT NULL,
    "City" TEXT NOT NULL,
    "State" TEXT NOT NULL,
    "ZipCode" TEXT NOT NULL,
    "PropertyType" TEXT NOT NULL,
    "MonthlyRent" decimal(18,2) NOT NULL,
    "Bedrooms" INTEGER NOT NULL,
    "Bathrooms" decimal(3,1) NOT NULL,
    "SquareFeet" INTEGER NOT NULL,
    "Description" TEXT NOT NULL,
    "IsAvailable" INTEGER NOT NULL,
    "LastRoutineInspectionDate" TEXT NULL,
    "NextRoutineInspectionDueDate" TEXT NULL,
    "RoutineInspectionIntervalMonths" INTEGER NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Properties_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
);

CREATE TABLE "Tenants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tenants" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "IdentificationNumber" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "PhoneNumber" TEXT NOT NULL,
    "DateOfBirth" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "EmergencyContactName" TEXT NOT NULL,
    "EmergencyContactPhone" TEXT NULL,
    "Notes" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Tenants_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
);

CREATE TABLE "ChecklistTemplateItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ChecklistTemplateItems" PRIMARY KEY AUTOINCREMENT,
    "ChecklistTemplateId" INTEGER NOT NULL,
    "ItemText" TEXT NOT NULL,
    "ItemOrder" INTEGER NOT NULL,
    "CategorySection" TEXT NULL,
    "SectionOrder" INTEGER NOT NULL,
    "IsRequired" INTEGER NOT NULL,
    "RequiresValue" INTEGER NOT NULL,
    "AllowsNotes" INTEGER NOT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId" FOREIGN KEY ("ChecklistTemplateId") REFERENCES "ChecklistTemplates" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CalendarEvents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CalendarEvents" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "StartOn" TEXT NOT NULL,
    "EndOn" TEXT NULL,
    "DurationMinutes" INTEGER NOT NULL,
    "EventType" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "Description" TEXT NULL,
    "PropertyId" INTEGER NULL,
    "Location" TEXT NULL,
    "Color" TEXT NOT NULL,
    "Icon" TEXT NOT NULL,
    "SourceEntityId" INTEGER NULL,
    "SourceEntityType" TEXT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_CalendarEvents_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE SET NULL
);

CREATE TABLE "ProspectiveTenants" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ProspectiveTenants" PRIMARY KEY AUTOINCREMENT,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "DateOfBirth" TEXT NULL,
    "IdentificationNumber" TEXT NULL,
    "IdentificationState" TEXT NULL,
    "Status" TEXT NOT NULL,
    "Source" TEXT NULL,
    "Notes" TEXT NULL,
    "InterestedPropertyId" INTEGER NULL,
    "DesiredMoveInDate" TEXT NULL,
    "FirstContactedOn" TEXT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_ProspectiveTenants_Properties_InterestedPropertyId" FOREIGN KEY ("InterestedPropertyId") REFERENCES "Properties" ("Id") ON DELETE SET NULL
);

CREATE TABLE "RentalApplications" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_RentalApplications" PRIMARY KEY AUTOINCREMENT,
    "ProspectiveTenantId" INTEGER NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "AppliedOn" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CurrentAddress" TEXT NOT NULL,
    "CurrentCity" TEXT NOT NULL,
    "CurrentState" TEXT NOT NULL,
    "CurrentZipCode" TEXT NOT NULL,
    "CurrentRent" decimal(18,2) NOT NULL,
    "LandlordName" TEXT NOT NULL,
    "LandlordPhone" TEXT NOT NULL,
    "EmployerName" TEXT NOT NULL,
    "JobTitle" TEXT NOT NULL,
    "MonthlyIncome" decimal(18,2) NOT NULL,
    "EmploymentLengthMonths" INTEGER NOT NULL,
    "Reference1Name" TEXT NOT NULL,
    "Reference1Phone" TEXT NOT NULL,
    "Reference1Relationship" TEXT NOT NULL,
    "Reference2Name" TEXT NULL,
    "Reference2Phone" TEXT NULL,
    "Reference2Relationship" TEXT NULL,
    "ApplicationFee" decimal(18,2) NOT NULL,
    "ApplicationFeePaid" INTEGER NOT NULL,
    "ApplicationFeePaidOn" TEXT NULL,
    "DenialReason" TEXT NULL,
    "DecidedOn" TEXT NULL,
    "DecisionBy" TEXT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_RentalApplications_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RentalApplications_ProspectiveTenants_ProspectiveTenantId" FOREIGN KEY ("ProspectiveTenantId") REFERENCES "ProspectiveTenants" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ApplicationScreenings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApplicationScreenings" PRIMARY KEY AUTOINCREMENT,
    "RentalApplicationId" INTEGER NOT NULL,
    "BackgroundCheckRequested" INTEGER NOT NULL,
    "BackgroundCheckRequestedOn" TEXT NULL,
    "BackgroundCheckPassed" INTEGER NULL,
    "BackgroundCheckCompletedOn" TEXT NULL,
    "BackgroundCheckNotes" TEXT NULL,
    "CreditCheckRequested" INTEGER NOT NULL,
    "CreditCheckRequestedOn" TEXT NULL,
    "CreditScore" INTEGER NULL,
    "CreditCheckPassed" INTEGER NULL,
    "CreditCheckCompletedOn" TEXT NULL,
    "CreditCheckNotes" TEXT NULL,
    "OverallResult" TEXT NOT NULL,
    "ResultNotes" TEXT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_ApplicationScreenings_RentalApplications_RentalApplicationId" FOREIGN KEY ("RentalApplicationId") REFERENCES "RentalApplications" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ChecklistItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ChecklistItems" PRIMARY KEY AUTOINCREMENT,
    "ChecklistId" INTEGER NOT NULL,
    "ItemText" TEXT NOT NULL,
    "ItemOrder" INTEGER NOT NULL,
    "CategorySection" TEXT NULL,
    "SectionOrder" INTEGER NOT NULL,
    "RequiresValue" INTEGER NOT NULL,
    "Value" TEXT NULL,
    "Notes" TEXT NULL,
    "PhotoUrl" TEXT NULL,
    "IsChecked" INTEGER NOT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_ChecklistItems_Checklists_ChecklistId" FOREIGN KEY ("ChecklistId") REFERENCES "Checklists" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Checklists" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Checklists" PRIMARY KEY AUTOINCREMENT,
    "PropertyId" INTEGER NULL,
    "LeaseId" INTEGER NULL,
    "ChecklistTemplateId" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "ChecklistType" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "CompletedBy" TEXT NULL,
    "CompletedOn" TEXT NULL,
    "DocumentId" INTEGER NULL,
    "GeneralNotes" TEXT NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Checklists_ChecklistTemplates_ChecklistTemplateId" FOREIGN KEY ("ChecklistTemplateId") REFERENCES "ChecklistTemplates" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Checklists_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Checklists_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Checklists_Leases_LeaseId" FOREIGN KEY ("LeaseId") REFERENCES "Leases" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Tours" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tours" PRIMARY KEY AUTOINCREMENT,
    "ProspectiveTenantId" INTEGER NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "ScheduledOn" TEXT NOT NULL,
    "DurationMinutes" INTEGER NOT NULL,
    "Status" TEXT NOT NULL,
    "Feedback" TEXT NULL,
    "InterestLevel" TEXT NULL,
    "ConductedBy" TEXT NULL,
    "ChecklistId" INTEGER NULL,
    "CalendarEventId" INTEGER NULL,
    "OrganizationId" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Tours_Checklists_ChecklistId" FOREIGN KEY ("ChecklistId") REFERENCES "Checklists" ("Id"),
    CONSTRAINT "FK_Tours_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Tours_ProspectiveTenants_ProspectiveTenantId" FOREIGN KEY ("ProspectiveTenantId") REFERENCES "ProspectiveTenants" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Documents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Documents" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "FileName" TEXT NOT NULL,
    "FileExtension" TEXT NOT NULL,
    "FileData" BLOB NOT NULL,
    "FilePath" TEXT NOT NULL,
    "ContentType" TEXT NOT NULL,
    "FileType" TEXT NOT NULL,
    "FileSize" INTEGER NOT NULL,
    "DocumentType" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "PropertyId" INTEGER NULL,
    "TenantId" INTEGER NULL,
    "LeaseId" INTEGER NULL,
    "InvoiceId" INTEGER NULL,
    "PaymentId" INTEGER NULL,
    "UploadedBy" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Documents_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Documents_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Documents_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Documents_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Documents_Leases_LeaseId" FOREIGN KEY ("LeaseId") REFERENCES "Leases" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Documents_Payments_PaymentId" FOREIGN KEY ("PaymentId") REFERENCES "Payments" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Leases" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Leases" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "TenantId" INTEGER NOT NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    "MonthlyRent" decimal(18,2) NOT NULL,
    "SecurityDeposit" decimal(18,2) NOT NULL,
    "Status" TEXT NOT NULL,
    "Terms" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "RenewalNotificationSent" INTEGER NULL,
    "RenewalNotificationSentOn" TEXT NULL,
    "RenewalReminderSentOn" TEXT NULL,
    "RenewalStatus" TEXT NULL,
    "RenewalOfferedOn" TEXT NULL,
    "RenewalResponseOn" TEXT NULL,
    "ProposedRenewalRent" decimal(18,2) NULL,
    "RenewalNotes" TEXT NULL,
    "PreviousLeaseId" INTEGER NULL,
    "DocumentId" INTEGER NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Leases_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Leases_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Leases_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Leases_Tenants_TenantId" FOREIGN KEY ("TenantId") REFERENCES "Tenants" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Inspections" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Inspections" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "UserId" TEXT NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "CalendarEventId" INTEGER NULL,
    "LeaseId" INTEGER NULL,
    "CompletedOn" TEXT NOT NULL,
    "InspectionType" TEXT NOT NULL,
    "InspectedBy" TEXT NULL,
    "ExteriorRoofGood" INTEGER NOT NULL,
    "ExteriorRoofNotes" TEXT NULL,
    "ExteriorGuttersGood" INTEGER NOT NULL,
    "ExteriorGuttersNotes" TEXT NULL,
    "ExteriorSidingGood" INTEGER NOT NULL,
    "ExteriorSidingNotes" TEXT NULL,
    "ExteriorWindowsGood" INTEGER NOT NULL,
    "ExteriorWindowsNotes" TEXT NULL,
    "ExteriorDoorsGood" INTEGER NOT NULL,
    "ExteriorDoorsNotes" TEXT NULL,
    "ExteriorFoundationGood" INTEGER NOT NULL,
    "ExteriorFoundationNotes" TEXT NULL,
    "LandscapingGood" INTEGER NOT NULL,
    "LandscapingNotes" TEXT NULL,
    "InteriorWallsGood" INTEGER NOT NULL,
    "InteriorWallsNotes" TEXT NULL,
    "InteriorCeilingsGood" INTEGER NOT NULL,
    "InteriorCeilingsNotes" TEXT NULL,
    "InteriorFloorsGood" INTEGER NOT NULL,
    "InteriorFloorsNotes" TEXT NULL,
    "InteriorDoorsGood" INTEGER NOT NULL,
    "InteriorDoorsNotes" TEXT NULL,
    "InteriorWindowsGood" INTEGER NOT NULL,
    "InteriorWindowsNotes" TEXT NULL,
    "KitchenAppliancesGood" INTEGER NOT NULL,
    "KitchenAppliancesNotes" TEXT NULL,
    "KitchenCabinetsGood" INTEGER NOT NULL,
    "KitchenCabinetsNotes" TEXT NULL,
    "KitchenCountersGood" INTEGER NOT NULL,
    "KitchenCountersNotes" TEXT NULL,
    "KitchenSinkPlumbingGood" INTEGER NOT NULL,
    "KitchenSinkPlumbingNotes" TEXT NULL,
    "BathroomToiletGood" INTEGER NOT NULL,
    "BathroomToiletNotes" TEXT NULL,
    "BathroomSinkGood" INTEGER NOT NULL,
    "BathroomSinkNotes" TEXT NULL,
    "BathroomTubShowerGood" INTEGER NOT NULL,
    "BathroomTubShowerNotes" TEXT NULL,
    "BathroomVentilationGood" INTEGER NOT NULL,
    "BathroomVentilationNotes" TEXT NULL,
    "HvacSystemGood" INTEGER NOT NULL,
    "HvacSystemNotes" TEXT NULL,
    "ElectricalSystemGood" INTEGER NOT NULL,
    "ElectricalSystemNotes" TEXT NULL,
    "PlumbingSystemGood" INTEGER NOT NULL,
    "PlumbingSystemNotes" TEXT NULL,
    "SmokeDetectorsGood" INTEGER NOT NULL,
    "SmokeDetectorsNotes" TEXT NULL,
    "CarbonMonoxideDetectorsGood" INTEGER NOT NULL,
    "CarbonMonoxideDetectorsNotes" TEXT NULL,
    "OverallCondition" TEXT NOT NULL,
    "GeneralNotes" TEXT NULL,
    "ActionItemsRequired" TEXT NULL,
    "DocumentId" INTEGER NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Inspections_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Inspections_Leases_LeaseId" FOREIGN KEY ("LeaseId") REFERENCES "Leases" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Inspections_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Invoices" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Invoices" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "LeaseId" INTEGER NOT NULL,
    "InvoiceNumber" TEXT NOT NULL,
    "InvoicedOn" TEXT NOT NULL,
    "DueOn" TEXT NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "Description" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "PaidOn" TEXT NULL,
    "AmountPaid" decimal(18,2) NOT NULL,
    "Notes" TEXT NOT NULL,
    "LateFeeAmount" decimal(18,2) NULL,
    "LateFeeApplied" INTEGER NULL,
    "LateFeeAppliedOn" TEXT NULL,
    "ReminderSent" INTEGER NULL,
    "ReminderSentOn" TEXT NULL,
    "DocumentId" INTEGER NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Invoices_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Invoices_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Invoices_Leases_LeaseId" FOREIGN KEY ("LeaseId") REFERENCES "Leases" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "MaintenanceRequests" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MaintenanceRequests" PRIMARY KEY AUTOINCREMENT,
    "OrganizationId" TEXT NOT NULL,
    "PropertyId" INTEGER NOT NULL,
    "CalendarEventId" INTEGER NULL,
    "LeaseId" INTEGER NULL,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "RequestType" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "RequestedBy" TEXT NOT NULL,
    "RequestedByEmail" TEXT NOT NULL,
    "RequestedByPhone" TEXT NOT NULL,
    "RequestedOn" TEXT NOT NULL,
    "ScheduledOn" TEXT NULL,
    "CompletedOn" TEXT NULL,
    "EstimatedCost" decimal(18,2) NOT NULL,
    "ActualCost" decimal(18,2) NOT NULL,
    "AssignedTo" TEXT NOT NULL,
    "ResolutionNotes" TEXT NOT NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_MaintenanceRequests_Leases_LeaseId" FOREIGN KEY ("LeaseId") REFERENCES "Leases" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_MaintenanceRequests_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Payments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Payments" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "InvoiceId" INTEGER NOT NULL,
    "PaidOn" TEXT NOT NULL,
    "Amount" decimal(18,2) NOT NULL,
    "PaymentMethod" TEXT NOT NULL,
    "Notes" TEXT NOT NULL,
    "DocumentId" INTEGER NULL,
    "CreatedOn" TEXT NOT NULL,
    "CreatedBy" TEXT NOT NULL,
    "LastModifiedOn" TEXT NULL,
    "LastModifiedBy" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_Payments_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Payments_Documents_DocumentId" FOREIGN KEY ("DocumentId") REFERENCES "Documents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Payments_Invoices_InvoiceId" FOREIGN KEY ("InvoiceId") REFERENCES "Invoices" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_ApplicationScreenings_OrganizationId" ON "ApplicationScreenings" ("OrganizationId");

CREATE INDEX "IX_ApplicationScreenings_OverallResult" ON "ApplicationScreenings" ("OverallResult");

CREATE UNIQUE INDEX "IX_ApplicationScreenings_RentalApplicationId" ON "ApplicationScreenings" ("RentalApplicationId");

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_CalendarEvents_EventType" ON "CalendarEvents" ("EventType");

CREATE INDEX "IX_CalendarEvents_OrganizationId" ON "CalendarEvents" ("OrganizationId");

CREATE INDEX "IX_CalendarEvents_PropertyId" ON "CalendarEvents" ("PropertyId");

CREATE INDEX "IX_CalendarEvents_SourceEntityId" ON "CalendarEvents" ("SourceEntityId");

CREATE INDEX "IX_CalendarEvents_SourceEntityType_SourceEntityId" ON "CalendarEvents" ("SourceEntityType", "SourceEntityId");

CREATE INDEX "IX_CalendarEvents_StartOn" ON "CalendarEvents" ("StartOn");

CREATE INDEX "IX_CalendarSettings_OrganizationId" ON "CalendarSettings" ("OrganizationId");

CREATE UNIQUE INDEX "IX_CalendarSettings_OrganizationId_EntityType" ON "CalendarSettings" ("OrganizationId", "EntityType");

CREATE INDEX "IX_ChecklistItems_ChecklistId" ON "ChecklistItems" ("ChecklistId");

CREATE INDEX "IX_Checklists_ChecklistTemplateId" ON "Checklists" ("ChecklistTemplateId");

CREATE INDEX "IX_Checklists_ChecklistType" ON "Checklists" ("ChecklistType");

CREATE INDEX "IX_Checklists_CompletedOn" ON "Checklists" ("CompletedOn");

CREATE INDEX "IX_Checklists_DocumentId" ON "Checklists" ("DocumentId");

CREATE INDEX "IX_Checklists_LeaseId" ON "Checklists" ("LeaseId");

CREATE INDEX "IX_Checklists_PropertyId" ON "Checklists" ("PropertyId");

CREATE INDEX "IX_Checklists_Status" ON "Checklists" ("Status");

CREATE INDEX "IX_ChecklistTemplateItems_ChecklistTemplateId" ON "ChecklistTemplateItems" ("ChecklistTemplateId");

CREATE INDEX "IX_ChecklistTemplates_Category" ON "ChecklistTemplates" ("Category");

CREATE INDEX "IX_ChecklistTemplates_OrganizationId" ON "ChecklistTemplates" ("OrganizationId");

CREATE INDEX "IX_Documents_InvoiceId" ON "Documents" ("InvoiceId");

CREATE INDEX "IX_Documents_LeaseId" ON "Documents" ("LeaseId");

CREATE INDEX "IX_Documents_PaymentId" ON "Documents" ("PaymentId");

CREATE INDEX "IX_Documents_PropertyId" ON "Documents" ("PropertyId");

CREATE INDEX "IX_Documents_TenantId" ON "Documents" ("TenantId");

CREATE INDEX "IX_Documents_UserId" ON "Documents" ("UserId");

CREATE INDEX "IX_Inspections_CompletedOn" ON "Inspections" ("CompletedOn");

CREATE INDEX "IX_Inspections_DocumentId" ON "Inspections" ("DocumentId");

CREATE INDEX "IX_Inspections_LeaseId" ON "Inspections" ("LeaseId");

CREATE INDEX "IX_Inspections_PropertyId" ON "Inspections" ("PropertyId");

CREATE INDEX "IX_Invoices_DocumentId" ON "Invoices" ("DocumentId");

CREATE UNIQUE INDEX "IX_Invoices_InvoiceNumber" ON "Invoices" ("InvoiceNumber");

CREATE INDEX "IX_Invoices_LeaseId" ON "Invoices" ("LeaseId");

CREATE INDEX "IX_Invoices_UserId" ON "Invoices" ("UserId");

CREATE INDEX "IX_Leases_DocumentId" ON "Leases" ("DocumentId");

CREATE INDEX "IX_Leases_PropertyId" ON "Leases" ("PropertyId");

CREATE INDEX "IX_Leases_TenantId" ON "Leases" ("TenantId");

CREATE INDEX "IX_Leases_UserId" ON "Leases" ("UserId");

CREATE INDEX "IX_MaintenanceRequests_LeaseId" ON "MaintenanceRequests" ("LeaseId");

CREATE INDEX "IX_MaintenanceRequests_Priority" ON "MaintenanceRequests" ("Priority");

CREATE INDEX "IX_MaintenanceRequests_PropertyId" ON "MaintenanceRequests" ("PropertyId");

CREATE INDEX "IX_MaintenanceRequests_RequestedOn" ON "MaintenanceRequests" ("RequestedOn");

CREATE INDEX "IX_MaintenanceRequests_Status" ON "MaintenanceRequests" ("Status");

CREATE INDEX "IX_Notes_CreatedBy" ON "Notes" ("CreatedBy");

CREATE UNIQUE INDEX "IX_OrganizationSettings_OrganizationId" ON "OrganizationSettings" ("OrganizationId");

CREATE INDEX "IX_Payments_DocumentId" ON "Payments" ("DocumentId");

CREATE INDEX "IX_Payments_InvoiceId" ON "Payments" ("InvoiceId");

CREATE INDEX "IX_Payments_UserId" ON "Payments" ("UserId");

CREATE INDEX "IX_Properties_Address" ON "Properties" ("Address");

CREATE INDEX "IX_Properties_UserId" ON "Properties" ("UserId");

CREATE INDEX "IX_ProspectiveTenants_Email" ON "ProspectiveTenants" ("Email");

CREATE INDEX "IX_ProspectiveTenants_InterestedPropertyId" ON "ProspectiveTenants" ("InterestedPropertyId");

CREATE INDEX "IX_ProspectiveTenants_OrganizationId" ON "ProspectiveTenants" ("OrganizationId");

CREATE INDEX "IX_ProspectiveTenants_Status" ON "ProspectiveTenants" ("Status");

CREATE INDEX "IX_RentalApplications_AppliedOn" ON "RentalApplications" ("AppliedOn");

CREATE INDEX "IX_RentalApplications_OrganizationId" ON "RentalApplications" ("OrganizationId");

CREATE INDEX "IX_RentalApplications_PropertyId" ON "RentalApplications" ("PropertyId");

CREATE UNIQUE INDEX "IX_RentalApplications_ProspectiveTenantId" ON "RentalApplications" ("ProspectiveTenantId");

CREATE INDEX "IX_RentalApplications_Status" ON "RentalApplications" ("Status");

CREATE UNIQUE INDEX "IX_Tenants_Email" ON "Tenants" ("Email");

CREATE UNIQUE INDEX "IX_Tenants_IdentificationNumber" ON "Tenants" ("IdentificationNumber");

CREATE INDEX "IX_Tenants_UserId" ON "Tenants" ("UserId");

CREATE INDEX "IX_Tours_ChecklistId" ON "Tours" ("ChecklistId");

CREATE INDEX "IX_Tours_OrganizationId" ON "Tours" ("OrganizationId");

CREATE INDEX "IX_Tours_PropertyId" ON "Tours" ("PropertyId");

CREATE INDEX "IX_Tours_ProspectiveTenantId" ON "Tours" ("ProspectiveTenantId");

CREATE INDEX "IX_Tours_ScheduledOn" ON "Tours" ("ScheduledOn");

CREATE INDEX "IX_Tours_Status" ON "Tours" ("Status");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251128175458_InitialCreate', '9.0.11');

-- Seed System Checklist Templates
INSERT INTO "ChecklistTemplates" ("Name", "Description", "Category", "IsSystemTemplate", "OrganizationId", "CreatedOn", "CreatedBy", "IsDeleted")
VALUES 
('Property Tour', 'Standard property showing checklist', 'Showing', 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
('Move-In', 'Move-in inspection checklist', 'MoveIn', 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
('Move-Out', 'Move-out inspection checklist', 'MoveOut', 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
('Open House', 'Open house event checklist', 'Showing', 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0);

-- Property Tour Checklist Items
INSERT INTO "ChecklistTemplateItems" ("ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "SectionOrder", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "IsDeleted")
VALUES
-- Arrival & Introduction (Section 1)
(1, 'Greeted prospect and verified appointment', 1, 'Arrival & Introduction', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Reviewed property exterior and curb appeal', 2, 'Arrival & Introduction', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed parking area/garage', 3, 'Arrival & Introduction', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Interior Tour (Section 2)
(1, 'Toured living room/common areas', 4, 'Interior Tour', 2, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed all bedrooms', 5, 'Interior Tour', 2, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed all bathrooms', 6, 'Interior Tour', 2, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Kitchen & Appliances (Section 3)
(1, 'Toured kitchen and demonstrated appliances', 7, 'Kitchen & Appliances', 3, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Explained which appliances are included', 8, 'Kitchen & Appliances', 3, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Utilities & Systems (Section 4)
(1, 'Explained HVAC system and thermostat controls', 9, 'Utilities & Systems', 4, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Reviewed utility responsibilities (tenant vs landlord)', 10, 'Utilities & Systems', 4, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed water heater location', 11, 'Utilities & Systems', 4, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Storage & Amenities (Section 5)
(1, 'Showed storage areas (closets, attic, basement)', 12, 'Storage & Amenities', 5, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed laundry facilities', 13, 'Storage & Amenities', 5, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Showed outdoor space (yard, patio, balcony)', 14, 'Storage & Amenities', 5, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Lease Terms (Section 6)
(1, 'Discussed monthly rent amount', 15, 'Lease Terms', 6, 1, 1, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Explained security deposit and move-in costs', 16, 'Lease Terms', 6, 1, 1, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Reviewed lease term length and start date', 17, 'Lease Terms', 6, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Explained pet policy', 18, 'Lease Terms', 6, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Next Steps (Section 7)
(1, 'Explained application process and requirements', 19, 'Next Steps', 7, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Reviewed screening process (background, credit check)', 20, 'Next Steps', 7, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Answered all prospect questions', 21, 'Next Steps', 7, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),

-- Assessment (Section 8)
(1, 'Prospect Interest Level', 22, 'Assessment', 8, 1, 1, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(1, 'Overall showing feedback and notes', 23, 'Assessment', 8, 1, 1, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0);

-- Move-In Checklist Items (Placeholder - to be expanded)
INSERT INTO "ChecklistTemplateItems" ("ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "SectionOrder", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "IsDeleted")
VALUES
(2, 'Document property condition', 1, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(2, 'Collect keys and access codes', 2, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(2, 'Review lease terms with tenant', 3, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0);

-- Move-Out Checklist Items (Placeholder - to be expanded)
INSERT INTO "ChecklistTemplateItems" ("ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "SectionOrder", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "IsDeleted")
VALUES
(3, 'Inspect property condition', 1, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(3, 'Collect all keys and access devices', 2, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(3, 'Document damages and needed repairs', 3, 'General', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0);

-- Open House Checklist Items (Placeholder - to be expanded)
INSERT INTO "ChecklistTemplateItems" ("ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "SectionOrder", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "IsDeleted")
VALUES
(4, 'Set up signage and directional markers', 1, 'Preparation', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(4, 'Prepare information packets', 2, 'Preparation', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0),
(4, 'Set up visitor sign-in sheet', 3, 'Preparation', 1, 1, 0, 1, 'SYSTEM', '2025-11-30T00:00:00Z', 'SYSTEM', 0);

COMMIT;

