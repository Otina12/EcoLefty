using Common.Shared;
using EcoLefty.Application;
using EcoLefty.Domain.Contracts;
using EcoLefty.Infrastructure;
using EcoLefty.Persistence.Context;
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

        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IServiceManager, ServiceManager>();
    }
}
