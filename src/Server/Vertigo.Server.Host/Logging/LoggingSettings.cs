namespace Vertigo.Server.Host.Logging;

internal static class LoggingSettings
{
    public const string DirectoryKey = "Logging:Directory";
    public const string ConsoleEnabledKey = "Logging:ConsoleEnabled";
    public const string TechnicalMinimumLevelKey = "Logging:TechnicalMinimumLevel";
    public const string FrameworkMinimumLevelKey = "Logging:FrameworkMinimumLevel";
    public const string EventCategoryProperty = "EventCategory";
    public const string SecurityCategory = "Security";
    public const long MaximumFileSizeBytes = 10L * 1024 * 1024;
    public const int TechnicalRetainedFileCount = 30;
    public const int SecurityRetainedFileCount = 60;
    public const int InitialDiagnosticLimit = 3;
    public const int SubsequentDiagnosticInterval = 100;
}
