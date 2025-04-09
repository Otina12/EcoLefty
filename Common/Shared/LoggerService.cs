using ILogger = Serilog.ILogger;

namespace EcoLefty.Application.Common.Logger;

public class LoggerService : ILoggerService
{
    private static ILogger _logger = null!;

    public LoggerService(ILogger logger)
    {
        _logger = logger;
    }

    public void LogInfo(string message) => _logger.Information(message);

    public void LogWarn(string message) => _logger.Warning(message);

    public void LogDebug(string message) => _logger.Debug(message);

    public void LogError(string message) => _logger.Error(message);

    public void LogFatal(string message) => _logger.Fatal(message);
}
