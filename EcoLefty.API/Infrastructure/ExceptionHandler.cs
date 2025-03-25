using EcoLefty.Application.Contracts;
using EcoLefty.Domain.Common.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.API.Infrastructure;

internal sealed class ExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILoggerService _logger;

    public ExceptionHandler(IProblemDetailsService problemDetailsService, ILoggerService logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
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

        LogException(status, exception);

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

    private void LogException(int statusCode, Exception exception)
    {
        var logMessage = $"[{exception.GetType().Name}] {exception.Message} | Source: {exception.TargetSite}";

        switch (statusCode)
        {
            case StatusCodes.Status400BadRequest:
            case StatusCodes.Status404NotFound:
            case StatusCodes.Status409Conflict:
                _logger.LogInfo(logMessage);
                break;
            default:
                _logger.LogError($"[Unhandled Error] {logMessage}");
                break;
        }
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
