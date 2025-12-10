using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    /// <summary>
    /// Audit log for workflow state transitions.
    /// Tracks all status changes with context and timestamp.
    /// </summary>
    public class WorkflowAuditLog : BaseModel
    {
        /// <summary>
        /// Type of entity (Application, Lease, MaintenanceRequest, etc.)
        /// </summary>
        public required string EntityType { get; set; }

        /// <summary>
        /// ID of the entity that transitioned
        /// </summary>
        public required Guid EntityId { get; set; }
        /// </summary>
        public string? FromStatus { get; set; }

        /// <summary>
        /// New status after transition
        /// </summary>
        public required string ToStatus { get; set; }

        /// <summary>
        /// Action that triggered the transition (e.g., "Submit", "Approve", "Deny")
        /// </summary>
        public required string Action { get; set; }

        /// <summary>
        /// Optional reason/notes for the transition
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// User who performed the action (from UserContextService)
        /// </summary>
        public required string PerformedBy { get; set; }

        /// <summary>
        /// When the action occurred
        /// </summary>
        public required DateTime PerformedOn { get; set; }

        /// <summary>
        /// Organization context for the workflow action
        /// </summary>
        public required Guid OrganizationId { get; set; }

        /// <summary>
        /// Additional context data (JSON serialized)
        /// </summary>
        public string? Metadata { get; set; }
    }
}
