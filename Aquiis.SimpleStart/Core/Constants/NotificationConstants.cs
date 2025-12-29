
namespace Aquiis.SimpleStart.Core.Constants
{
    public static class NotificationConstants
    {
        public static class Types
        {
            public const string Info = "Info";
            public const string Warning = "Warning";
            public const string Error = "Error";
            public const string Success = "Success";
        }

        public static class Categories
        {
            public const string Lease = "Lease";
            public const string Payment = "Payment";
            public const string Maintenance = "Maintenance";
            public const string Application = "Application";
            public const string Property = "Property";
            public const string Inspection = "Inspection";
            public const string Document = "Document";
            public const string System = "System";
        }

        public static class Templates
        {
            // Lease notifications
            public const string LeaseExpiring90Days = "lease_expiring_90";
            public const string LeaseExpiring60Days = "lease_expiring_60";
            public const string LeaseExpiring30Days = "lease_expiring_30";
            public const string LeaseActivated = "lease_activated";
            public const string LeaseTerminated = "lease_terminated";

            // Payment notifications
            public const string PaymentDueReminder = "payment_due_reminder";
            public const string PaymentReceived = "payment_received";
            public const string PaymentLate = "payment_late";
            public const string LateFeeApplied = "late_fee_applied";

            // Maintenance notifications
            public const string MaintenanceRequestCreated = "maintenance_created";
            public const string MaintenanceRequestAssigned = "maintenance_assigned";
            public const string MaintenanceRequestStarted = "maintenance_started";
            public const string MaintenanceRequestCompleted = "maintenance_completed";

            // Application notifications
            public const string ApplicationSubmitted = "application_submitted";
            public const string ApplicationUnderReview = "application_under_review";
            public const string ApplicationApproved = "application_approved";
            public const string ApplicationRejected = "application_rejected";

            // Inspection notifications
            public const string InspectionScheduled = "inspection_scheduled";
            public const string InspectionCompleted = "inspection_completed";

            // Document notifications
            public const string DocumentUploaded = "document_uploaded";
            public const string DocumentExpiring = "document_expiring";
        }
    }
}