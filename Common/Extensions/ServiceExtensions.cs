using Common.Shared;
using EcoLefty.Application;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens;
using EcoLefty.Application.Contracts;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities.Identity;
using EcoLefty.Infrastructure;
using EcoLefty.Infrastructure.Repositories;
using EcoLefty.Persistence.Context;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenService, TokenService>();

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

}
