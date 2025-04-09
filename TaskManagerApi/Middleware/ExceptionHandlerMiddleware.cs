using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace TaskManagerAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetails = new ProblemDetails();

            switch (exception)
            {
                case NotFoundException notFoundException:
                    problemDetails.Title = "Resource not found";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = notFoundException.Message;
                    _logger.LogWarning(notFoundException, "Resource not found");
                    break;

                case BusinessException businessException:
                    problemDetails.Title = "Business rule violation";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = businessException.Message;
                    _logger.LogWarning(businessException, "Business rule violated");
                    break;

                case ValidationException validationException:
                    problemDetails.Title = "Validation errors";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = "Check the errors for more details";
                    problemDetails.Extensions["errors"] = validationException.Value;
                    _logger.LogWarning(validationException, "Validation errors");
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Detail = unauthorizedAccessException.Message;
                    _logger.LogWarning(unauthorizedAccessException, "Unauthorized access");
                    break;

                default:
                    problemDetails.Title = "Internal server error";
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Detail = "An unexpected error occurred";
                    _logger.LogError(exception, "Unhandled error");
                    break;
            }

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
