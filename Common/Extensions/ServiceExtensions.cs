using EcoLefty.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EcoLeftyDbContext>(options =>
            options.UseSqlServer(connectionString)
            );
    }
}
