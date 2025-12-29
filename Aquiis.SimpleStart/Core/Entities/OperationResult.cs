namespace Aquiis.SimpleStart.Core.Entities
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static OperationResult SuccessResult(string message = "Operation completed successfully")
        {
            return new OperationResult { Success = true, Message = message };
        }

        public static OperationResult FailureResult(string message, List<string>? errors = null)
        {
            return new OperationResult 
            { 
                Success = false, 
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}