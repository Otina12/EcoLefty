using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace EcoLefty.Workers;

public static class DependencyInjection
{
    public static IServiceCollection AddQuartzScheduler(this IServiceCollection services)
    {
        services.AddQuartz();

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    public static IServiceCollectionQuartzConfigurator AddOfferArchiverWorkerJob(this IServiceCollectionQuartzConfigurator builder)
    {
        var jobKey = new JobKey("OfferArchiverWorker");

        builder.AddJob<OfferStatusUpdaterWorker>(opts => opts.WithIdentity(jobKey));
        builder.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("OfferArchiverWorker-trigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(2)
                .RepeatForever()));

        return builder;
    }

    public static IServiceCollectionQuartzConfigurator AddHealthCheckWorkerJob(this IServiceCollectionQuartzConfigurator builder)
    {
        var jobKey = new JobKey("HealthCheckWorker");

        builder.AddJob<HealthCheckWorker>(opts => opts.WithIdentity(jobKey));
        builder.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("HealthCheckWorker-trigger")
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever()));

        return builder;
    }
}