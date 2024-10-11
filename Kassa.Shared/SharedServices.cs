using Kassa.Shared.Logging;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Serilog;

namespace Kassa.Shared;
public static class SharedServices
{
    public static void AddRcsLoggers(this IServiceCollection services, string? path = null, LogEventLevel logEventLevel = LogEventLevel.Debug)
    {
        path ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "Logs.txt");

        const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} ({SourceContext})[{Level:u3}]: {Message:lj}{NewLine}{Exception}";  

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logEventLevel)
            .WriteTo.File(path, restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day, outputTemplate: template)
            .WriteTo.Debug(restrictedToMinimumLevel: LogEventLevel.Debug, outputTemplate: template)
            .WriteTo.Logger(new ObservableLogger())
            .CreateLogger();

        // Note: use self logger instead splat logger
        Locator.CurrentMutable.UseSerilogFullLogger();

        services.AddLogging();
        services.AddSerilog(Log.Logger);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="initialJsonFileName">In that json file the key "Environment"</param>
    /// <returns></returns>
    public static IConfiguration AddRcsConfiguration(this IConfigurationBuilder configurationBuilder, string initialJsonFileName = "appsettings")
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddJsonFile(Path.Combine(basePath, $"{initialJsonFileName}.local.json"), optional: true);

        var config = configurationBuilder.Build();

        Locator.CurrentMutable.RegisterConstant<IConfiguration>(config);

        return config;
    }
}
