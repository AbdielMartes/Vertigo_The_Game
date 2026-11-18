using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Vertigo.Server.Host.Logging;

internal static class ServerLoggingConfiguration
{
    private static int _diagnosticCount;

    public static Logger CreateLogger(IConfiguration configuration, IHostEnvironment environment)
    {
        string directory = GetBaseDirectory(configuration, environment.ContentRootPath);
        string technicalDirectory = Path.Combine(directory, "Technical");
        string securityDirectory = Path.Combine(directory, "Security");
        LogEventLevel technicalLevel = GetLevel(configuration, LoggingSettings.TechnicalMinimumLevelKey,
            LogEventLevel.Warning);
        LogEventLevel frameworkLevel = GetLevel(configuration, LoggingSettings.FrameworkMinimumLevelKey,
            environment.IsDevelopment() ? LogEventLevel.Information : LogEventLevel.Warning);
        bool consoleEnabled = configuration.GetValue(LoggingSettings.ConsoleEnabledKey, environment.IsDevelopment());
        LogEventLevel consoleLevel = LogEventLevel.Information;
        if (technicalLevel < consoleLevel)
        {
            consoleLevel = technicalLevel;
        }
        LogEventLevel rootLevel = consoleLevel < frameworkLevel ? consoleLevel : frameworkLevel;

        SelfLog.Enable(ReportInternalFailure);
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(rootLevel)
            .MinimumLevel.Override("Microsoft", frameworkLevel)
            .MinimumLevel.Override("System", frameworkLevel)
            .Enrich.FromLogContext();

        bool technicalAvailable = TryCreateDirectory(technicalDirectory);
        if (technicalAvailable)
        {
            technicalAvailable = TryConfigureTechnicalFile(loggerConfiguration, technicalDirectory, technicalLevel);
        }

        if (!technicalAvailable)
        {
            Console.Error.WriteLine("Technical log destination unavailable; using stderr for technical events.");
        }

        if (consoleEnabled || !technicalAvailable)
        {
            LogEventLevel minimumLevel = technicalAvailable ? consoleLevel : technicalLevel;
            loggerConfiguration.WriteTo.Logger(technical => technical
                .Filter.ByExcluding(IsSecurityEvent)
                .Filter.ByIncludingOnly(logEvent => logEvent.Level >= minimumLevel)
                .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose));
        }

        if (TryCreateDirectory(securityDirectory))
        {
            TryConfigureSecurityFile(loggerConfiguration, securityDirectory);
        }
        else
        {
            Console.Error.WriteLine("Security log destination unavailable; security events may be missing.");
        }

        return loggerConfiguration.CreateLogger();
    }

    private static bool TryConfigureTechnicalFile(
        LoggerConfiguration configuration, string directory, LogEventLevel minimumLevel)
    {
        bool success = true;
        string path = Path.Combine(directory, "technical-.jsonl");

        try
        {
            configuration.WriteTo.Logger(technical => technical
                .Filter.ByExcluding(IsSecurityEvent)
                .Filter.ByIncludingOnly(logEvent => logEvent.Level >= minimumLevel)
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true), path,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: LoggingSettings.MaximumFileSizeBytes,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: LoggingSettings.TechnicalRetainedFileCount));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            success = false;
        }

        return success;
    }

    private static void TryConfigureSecurityFile(LoggerConfiguration configuration, string directory)
    {
        string path = Path.Combine(directory, "security-.jsonl");

        try
        {
            configuration.WriteTo.Logger(security => security
                .Filter.ByIncludingOnly(IsSecurityEvent)
                .WriteTo.File(
                    new JsonFormatter(renderMessage: true), path,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: LoggingSettings.MaximumFileSizeBytes,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: LoggingSettings.SecurityRetainedFileCount));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            Console.Error.WriteLine("Security log destination unavailable; security events may be missing.");
        }
    }

    private static bool TryCreateDirectory(string directory)
    {
        bool success = true;

        try
        {
            Directory.CreateDirectory(directory);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                   or NotSupportedException)
        {
            success = false;
        }

        return success;
    }

    private static string GetBaseDirectory(IConfiguration configuration, string contentRootPath)
    {
        string? configuredDirectory = configuration[LoggingSettings.DirectoryKey];
        string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Vertigo", "Server", "Logs");

        if (!string.IsNullOrWhiteSpace(configuredDirectory))
        {
            if (Path.IsPathRooted(configuredDirectory))
            {
                directory = Path.GetFullPath(configuredDirectory);
            }
            else
            {
                directory = Path.GetFullPath(configuredDirectory, contentRootPath);
            }
        }

        return directory;
    }

    private static LogEventLevel GetLevel(
        IConfiguration configuration, string key, LogEventLevel defaultLevel)
    {
        LogEventLevel level = defaultLevel;
        string? configured = configuration[key];

        if (Enum.TryParse(configured, true, out LogEventLevel parsed) && Enum.IsDefined(parsed))
        {
            level = parsed;
        }

        return level;
    }

    private static bool IsSecurityEvent(LogEvent logEvent)
    {
        bool isSecurity = false;

        if (logEvent.Properties.TryGetValue(LoggingSettings.EventCategoryProperty, out LogEventPropertyValue? value)
            && value is ScalarValue { Value: string category })
        {
            isSecurity = string.Equals(category, LoggingSettings.SecurityCategory, StringComparison.Ordinal);
        }

        return isSecurity;
    }

    private static void ReportInternalFailure(string message)
    {
        int count = Interlocked.Increment(ref _diagnosticCount);
        if (count <= LoggingSettings.InitialDiagnosticLimit
            || count % LoggingSettings.SubsequentDiagnosticInterval == 0)
        {
            Console.Error.WriteLine("Serilog reported a write or storage problem; some log events may be missing.");
        }
    }
}
