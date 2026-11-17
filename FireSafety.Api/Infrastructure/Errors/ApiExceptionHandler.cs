using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FireSafety.Api.Infrastructure.Errors;

/// <summary>Преобразува бизнес и SQL грешките в ProblemDetails отговори.</summary>
public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            AppException app => (app.StatusCode, app.Message),
            SqlException { Number: 50007 } sql => (StatusCodes.Status400BadRequest, sql.Message),
            SqlException { Number: >= 50001 and <= 50099 } sql => (StatusCodes.Status409Conflict, sql.Message),
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                (StatusCodes.Status409Conflict, "Такъв запис вече съществува."),
            _ => (StatusCodes.Status500InternalServerError, "Възникна неочаквана грешка.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Необработена грешка при {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError && environment.IsDevelopment()
                    ? exception.Message
                    : null
            }
        });
    }
}
