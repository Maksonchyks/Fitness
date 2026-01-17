using FitnessApp.Identity.API.Common.Models;
using FitnessApp.Identity.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;
using ApplicationException = FitnessApp.Identity.Application.Common.Exceptions.ApplicationException;

namespace FitnessApp.Identity.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Path = context.Request.Path,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = "VALIDATION_ERROR";
                    errorResponse.Message = validationEx.Message;
                    errorResponse.ValidationErrors = validationEx.Errors;
                    break;

                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.ErrorCode = "NOT_FOUND";
                    errorResponse.Message = notFoundEx.Message;
                    break;

                case ConflictException conflictEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.ErrorCode = "CONFLICT";
                    errorResponse.Message = conflictEx.Message;
                    break;

                case UnauthorizedException unauthorizedEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    errorResponse.Message = unauthorizedEx.Message;
                    break;

                case ForbiddenException forbiddenEx:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.ErrorCode = "FORBIDDEN";
                    errorResponse.Message = forbiddenEx.Message;
                    break;

                case ApplicationException appEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = appEx.ErrorCode;
                    errorResponse.Message = appEx.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                    errorResponse.Message = "An unexpected error occurred";

                    if (_environment.IsDevelopment())
                    {
                        errorResponse.Details = exception.ToString();
                    }
                    break;
            }

            if (response.StatusCode >= 500)
            {
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
            }

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }
}
