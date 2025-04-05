using Microsoft.Extensions.Logging;
using Quartz;

namespace EcoLefty.Workers;

public class HealthCheckWorker : IJob
{
    private readonly ILogger<HealthCheckWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HealthCheckWorker(
        ILogger<HealthCheckWorker> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Executing health check worker");

        try
        {
            var client = _httpClientFactory.CreateClient("HealthCheck");
            var response = await client.GetAsync("/health");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Health check status: {Status}, Details: {Details}",
                    response.StatusCode, content);

                if (content.Contains("Unhealthy"))
                {
                    _logger.LogWarning("Health check returned UNHEALTHY status: {Details}", content);
                }
            }
            else
            {
                _logger.LogError("Health check failed with status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing health check worker");
        }
    }
}