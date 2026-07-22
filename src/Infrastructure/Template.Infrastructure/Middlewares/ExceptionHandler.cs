using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Template.Shared.Base.Response;
using Template.Shared.Exceptions;

namespace Template.Infrastructure.Middlewares;

/// <summary>
/// Turns the exception types in <c>Template.Shared.Exceptions</c> into HTTP
/// responses. Those carry messages written for end users; anything else is a
/// bug, and its message may contain connection strings, file paths or SQL, so
/// it is logged and replaced with a generic one.
/// </summary>
public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    private const string UnhandledMessage = "An unexpected error occurred.";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, UnhandledMessage)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(ServiceResponse.Failure(message, statusCode), cancellationToken);
        return true;
    }
}
