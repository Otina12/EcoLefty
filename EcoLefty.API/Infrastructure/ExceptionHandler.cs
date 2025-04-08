using EcoLefty.Application.Common.Logger;
using EcoLefty.Domain.Common.Exceptions.Base;
using FluentValidation;
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

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            await HandleValidationException(httpContext, validationException, cancellationToken);
            return true;
        }

        int status = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            AlreadyExistsException => StatusCodes.Status409Conflict,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            BadRequestException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        LogException(status, exception);

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = GetTitleForStatus(status),
            Status = status,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private async Task HandleValidationException(
        HttpContext httpContext,
        ValidationException validationException,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var error in validationException.Errors)
        {
            string propertyName = string.IsNullOrEmpty(error.PropertyName)
                ? "General"
                : error.PropertyName;

            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new[] { error.ErrorMessage };
            }
            else
            {
                var currentErrors = errors[propertyName].ToList();
                currentErrors.Add(error.ErrorMessage);
                errors[propertyName] = currentErrors.ToArray();
            }
        }

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = "ValidationException",
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest
        };

        LogException(StatusCodes.Status400BadRequest, validationException);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
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