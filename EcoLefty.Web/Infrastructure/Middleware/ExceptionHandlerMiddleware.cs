using EcoLefty.Application.Common.Logger;
using EcoLefty.Domain.Common.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;

namespace EcoLefty.Web.Infrastructure.Middleware;

public static class ExceptionHandlerMiddleware
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error ?? new Exception("An unknown error occurred.");

                await HandleException(context, exception, logger);
            });
        });
    }

    private static Task HandleException(HttpContext context, Exception exception, ILoggerService logger)
    {
        int statusCode;

        switch (exception)
        {
            case NotFoundException:
                statusCode = 404;
                break;
            case AlreadyExistsException:
                statusCode = 409;
                break;
            case UnauthorizedException:
                statusCode = 401;
                break;
            case ForbiddenException:
                statusCode = 403;
                break;
            case BadRequestException:
            case ArgumentException:
                statusCode = 400;
                break;
            default:
                statusCode = 500;
                break;
        };

        var logMessage = $"[{exception.GetType().Name}] {exception.Message} | Source: {exception.TargetSite}";
        if (statusCode == 500)
            logger.LogError($"[Unhandled Error] {logMessage}");
        else
            logger.LogInfo(logMessage);

        string safeMessage = exception.Message;
        if (safeMessage.Length > 100)
            safeMessage = safeMessage.Substring(0, 100) + "...";

        context.Response.Redirect($"/Error/{statusCode}?message={Uri.EscapeDataString(safeMessage)}");
        return Task.CompletedTask;
    }
}
