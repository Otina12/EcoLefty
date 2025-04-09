using Common.Shared;
using EcoLefty.Application;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Common.Images;
using EcoLefty.Application.Common.Logger;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Infrastructure;
using EcoLefty.Infrastructure.Repositories;
using EcoLefty.Infrastructure.Repositories.Common;
using EcoLefty.Persistence.Context;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using Serilog;

namespace Common.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EcoLeftyDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.EcoLefty)
            ));
    }

    public static void ConfigureHttpLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(options =>
        {
            options.LoggingFields =
                HttpLoggingFields.RequestPath |
                HttpLoggingFields.RequestBody |
                HttpLoggingFields.ResponseStatusCode |
                HttpLoggingFields.ResponseBody |
                HttpLoggingFields.Duration;
            options.MediaTypeOptions.AddText("application/json");
            options.MediaTypeOptions.AddText("text/plain");
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
            options.CombineLogs = true;
        });
    }

    public static void ConfigureSerilogILogger(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
    }

    public static void ConfigureJwtSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ITransactionWrapper, TransactionWrapper>();

        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IServiceManager, ServiceManager>();
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<Account, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireUppercase = true;
            o.Password.RequireNonAlphanumeric = true;
            o.Password.RequiredLength = 8;
            o.SignIn.RequireConfirmedAccount = false;
            o.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<EcoLeftyDbContext>();
    }

    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: configuration.GetConnectionString("DefaultConnection")!,
                    healthQuery: "SELECT 1;",
                    //healthQuery: "Test to check that health check works (should return unhealthy)",
                    name: "sql",
                    failureStatus: HealthStatus.Unhealthy);
    }

    /// <summary>
    /// WARNING: This should not allow all resources in production
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        });
    }

    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(EcoLefty.Application.AssemblyReference).Assembly);
    }

    public static void ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}
