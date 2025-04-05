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

    public static IServiceCollection AddOfferArchiverWorkerJob(this IServiceCollection services)
    {
        services.AddQuartz(config =>
        {
            var jobKey = new JobKey("OfferArchiverWorker");

            config.AddJob<OfferStatusUpdaterWorker>(opts => opts.WithIdentity(jobKey));

            config.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("OfferArchiverWorker-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(2)
                    .RepeatForever()));
        });

        return services;
    }

}