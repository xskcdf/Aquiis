namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    /// <summary>
    /// Interface for implementing state machines that validate workflow transitions.
    /// </summary>
    /// <typeparam name="TStatus">Enum type representing workflow statuses</typeparam>
    public interface IWorkflowState<TStatus> where TStatus : Enum
    {
        /// <summary>
        /// Validates if a transition from one status to another is allowed.
        /// </summary>
        /// <param name="fromStatus">Current status (can be null for initial creation)</param>
        /// <param name="toStatus">Target status</param>
        /// <returns>True if transition is valid</returns>
        bool IsValidTransition(TStatus fromStatus, TStatus toStatus);

        /// <summary>
        /// Gets all valid next statuses from the current status.
        /// </summary>
        /// <param name="currentStatus">Current status</param>
        /// <returns>List of valid next statuses</returns>
        List<TStatus> GetValidNextStates(TStatus currentStatus);

        /// <summary>
        /// Gets a human-readable reason why a transition is invalid.
        /// </summary>
        /// <param name="fromStatus">Current status</param>
        /// <param name="toStatus">Target status</param>
        /// <returns>Error message explaining why transition is invalid</returns>
        string GetInvalidTransitionReason(TStatus fromStatus, TStatus toStatus);
    }
}
