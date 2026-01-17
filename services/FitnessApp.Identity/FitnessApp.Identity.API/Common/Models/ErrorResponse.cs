namespace FitnessApp.Identity.API.Common.Models
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Path { get; set; }
        public IDictionary<string, string[]>? ValidationErrors { get; set; }

        public ErrorResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public ErrorResponse(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class ValidationErrorResponse : ErrorResponse
    {
        public IDictionary<string, string[]> Errors { get; set; }

        public ValidationErrorResponse(IDictionary<string, string[]> errors)
            : base("VALIDATION_ERROR", "One or more validation errors occurred")
        {
            Errors = errors;
        }
    }
}
