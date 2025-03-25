using EcoLefty.Domain.Common.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.API.Infrastructure.Middlewares;

internal sealed class ExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    // private readonly ILogger<ExceptionHandler> _logger; // Optional logger

    public ExceptionHandler(IProblemDetailsService problemDetailsService /*, ILogger<ExceptionHandler> logger */)
    {
        _problemDetailsService = problemDetailsService;
        // _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            AlreadyExistsException => StatusCodes.Status409Conflict,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = GetTitleForStatus(status),
            Type = exception.GetType().Name,
            Detail = exception.Message
        };

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static string GetTitleForStatus(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        404 => "Not Found",
        409 => "Conflict",
        500 => "Internal Server Error",
        _ => "Unexpected Error"
    };
}
