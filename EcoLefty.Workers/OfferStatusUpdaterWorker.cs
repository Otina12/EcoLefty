using EcoLefty.Application;
using Microsoft.Extensions.Logging;
using Quartz;

public class OfferStatusUpdaterWorker : IJob
{
    private readonly ILogger<OfferStatusUpdaterWorker> _logger;
    private readonly IServiceManager _serviceManager;

    public OfferStatusUpdaterWorker(
        ILogger<OfferStatusUpdaterWorker> logger,
        IServiceManager serviceManager)
    {
        _logger = logger;
        _serviceManager = serviceManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing offer archiver worker");
        await _serviceManager.OfferService.UpdateStatuses(context.CancellationToken);
    }
}