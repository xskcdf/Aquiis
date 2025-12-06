namespace Aquiis.SimpleStart.Application.Services.Workflows
{
    /// <summary>
    /// Standard result object for workflow operations.
    /// Provides success/failure status, error messages, and metadata.
    /// </summary>
    public class WorkflowResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();

        public static WorkflowResult Ok(string message = "Operation completed successfully")
        {
            return new WorkflowResult
            {
                Success = true,
                Message = message
            };
        }

        public static WorkflowResult Fail(string error)
        {
            return new WorkflowResult
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        public static WorkflowResult Fail(List<string> errors)
        {
            return new WorkflowResult
            {
                Success = false,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Workflow result with typed data payload.
    /// Used when operation returns a created/updated entity.
    /// </summary>
    public class WorkflowResult<T> : WorkflowResult
    {
        public T? Data { get; set; }

        public static WorkflowResult<T> Ok(T data, string message = "Operation completed successfully")
        {
            return new WorkflowResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public new static WorkflowResult<T> Fail(string error)
        {
            return new WorkflowResult<T>
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        public new static WorkflowResult<T> Fail(List<string> errors)
        {
            return new WorkflowResult<T>
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
