using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Coderaw.Settings.Extensions.Handlers
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError("The following error occured: {ErrorMessage}", exception.StackTrace);
            _logger.LogError("The following error occured: {ErrorMessage}", exception.Message);
            _logger.LogError(exception, "An error occurred: {Message}", exception.InnerException?.Message ?? "No inner exception");

            var code = exception switch
            {
                BadHttpRequestException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                HttpRequestException => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError,
            };

            var errors = new List<string>
            {
                exception.Message + " - " + exception.StackTrace
            };

            foreach (var key in exception.Data.Keys)
            {
                _logger.LogError("The following error occured: {ErrorMessage}", exception.Data[key]);
                _logger.LogError(exception, "More details can be find following the exception");
                errors.Add(exception.Data[key]?.ToString() ?? "");
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)code,
                Title = string.Join(",", errors)
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
