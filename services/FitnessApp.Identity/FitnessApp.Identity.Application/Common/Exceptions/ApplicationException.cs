using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Identity.Application.Common.Exceptions
{
    public class ApplicationException : Exception
    {
        public string ErrorCode { get; }
        public string ErrorMessage { get; }
        public ApplicationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
        public ApplicationException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorMessage = message;
        }
    }
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message) : base("VALIDATION_ERROR", message)
        {
        }
        public ValidationException(IDictionary<string, string[]> errors) : base("VALIDATION_ERROR", "One or more validation errors occurred")
        {
            Errors = errors;
        }
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base("NOT_FOUND", $"Entity '{name}' ({key}) was not found") { }
    }

    public class ConflictException : ApplicationException
    {
        public ConflictException(string message)
            : base("CONFLICT", message) { }
    }

    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message)
            : base("UNAUTHORIZED", message) { }
    }

    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message)
            : base("FORBIDDEN", message) { }
    }
}
