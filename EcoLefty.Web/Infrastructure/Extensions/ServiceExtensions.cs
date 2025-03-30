using FluentValidation;
using FluentValidation.AspNetCore;

namespace EcoLefty.API.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<EcoLefty.Application.AssemblyReference>();
    }
    //public static void ConfigureSerilogILogger(this ConfigureHostBuilder host)
    //{
    //    host.UseSerilog((context, loggerConfig) =>
    //        loggerConfig.ReadFrom.Configuration(context.Configuration));
    //}

    //public static void ConfigureLoggerService(this IServiceCollection services)
    //{
    //    services.AddSingleton<ILoggerService, LoggerService>();
    //}

    //public static void ConfigureHttpLogging(this IServiceCollection services)
    //{
    //    services.AddHttpLogging(options =>
    //    {
    //        options.LoggingFields =
    //            HttpLoggingFields.RequestPath |
    //            HttpLoggingFields.RequestBody |
    //            HttpLoggingFields.ResponseStatusCode |
    //            HttpLoggingFields.ResponseBody |
    //            HttpLoggingFields.Duration;
    //        options.MediaTypeOptions.AddText("application/json");
    //        options.MediaTypeOptions.AddText("text/plain");
    //        options.RequestBodyLogLimit = 4096;
    //        options.ResponseBodyLogLimit = 4096;
    //        options.CombineLogs = true;
    //    });
    //}

    //public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    //{
    //    app.UseMiddleware<RequestContextLoggingMiddleware>();
    //    return app;
    //}
}
