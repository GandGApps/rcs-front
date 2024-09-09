using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared.Logging;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Serilog;

namespace Kassa.Shared;
public static class SharedServices
{
    /// <summary>
    /// Retrieves a required service of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <param name="services">The read-only dependency resolver from which to retrieve the service.</param>
    /// <returns>The requested service of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver services)
    {
        return services.GetService<T>() ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }

    public static T GetRequiredService<T>(this IServiceProvider serviceProvider)
    {
        return (T?)serviceProvider.GetService(typeof(T)) ?? throw new InvalidOperationException($"The service of type {typeof(T)} is not registered.");
    }

    public static void AddLoggers(string? path = null, LogEventLevel logEventLevel = LogEventLevel.Debug)
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
    }

    public static async void RegisterConstantAndDiagnose<T>(this IMutableDependencyResolver services, T service)
    {
        if (service is IDevelopmentDiagnostics serviceDiagnose)
        {
            if (await serviceDiagnose.CheckService())
            {
                services.RegisterConstant(service);
            }
        }
        else
        {
            services.RegisterConstant(service);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="initialJsonFileName">In that json file the key "Environment"</param>
    /// <returns></returns>
    public static IConfiguration AddConfiguration(string initialJsonFileName, Action<ConfigurationBuilder>? builder = null)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile(Path.Combine(basePath, $"{initialJsonFileName}.json"), optional: false);

        var tempConfig = configurationBuilder.Build();

        var environment = tempConfig.GetValue("Environment", "Production");

        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddJsonFile(Path.Combine(basePath, $"{initialJsonFileName}.{environment}.json"), optional: true);
        configurationBuilder.AddJsonFile(Path.Combine(basePath, $"{initialJsonFileName}.local.json"), optional: true);

        builder?.Invoke(configurationBuilder);

        var config = configurationBuilder.Build();

        ServiceLocatorBuilder.AddService<IConfiguration>(() => config);
        Locator.CurrentMutable.RegisterConstant<IConfiguration>(config);

        RcsKassa.Configuration = config;

        return config;
    }
}
