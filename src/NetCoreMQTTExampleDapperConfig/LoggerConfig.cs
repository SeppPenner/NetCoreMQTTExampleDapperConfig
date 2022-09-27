namespace NetCoreMQTTExampleDapperConfig;

public static class LoggerConfig
{
    public static LoggerConfiguration GetLoggerConfiguration(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException(nameof(type), "type of logger must be given");
        }

        // set up logging for data frame output
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("LoggerType", type);
    }
}
