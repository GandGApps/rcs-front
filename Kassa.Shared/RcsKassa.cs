﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kassa.Shared;
public static class RcsKassa
{
    public const string ProductionName = "Production";
    public const string DevelopmentName = "Development";

    public static IHost Host
    {
        get; set;
    } = null!;

    public static IConfiguration Configuration => Host.Services.GetRequiredService<IConfiguration>();

    public static readonly CultureInfo RuCulture = new("ru-RU");

    public static string EnvironmentName => Configuration.GetValue<string>("Environment") ?? ProductionName;

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static string LogsPath => Path.Combine(BasePath, "logs", "Logs.txt");

    public static IServiceProvider ServiceProvider => Host.Services;
    public static IServiceScope? ScopedServices
    {
        get; private set;
    }

    public static bool IsDevelopment => string.Equals(EnvironmentName, DevelopmentName, StringComparison.InvariantCultureIgnoreCase);
    public static bool IsProduction => string.Equals(EnvironmentName, ProductionName, StringComparison.InvariantCultureIgnoreCase);

    public static T? GetService<T>() where T : class
    {
        if (ScopedServices is IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetService<T>();
        }

        return ServiceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>() where T: notnull
    {
        var type = typeof(T);

        if (ScopedServices is IServiceScope serviceScope)
        {
            return serviceScope.ServiceProvider.GetRequiredService<T>();
        }

        return ServiceProvider.GetRequiredService<T>();
    }

    public static async ValueTask ActivateScope()
    {
        var scope = ServiceProvider.CreateScope();

        await DisposeScope();

        var scopeActivator = new ScopeActivator(scope);

        await scopeActivator.Activate();

        ScopedServices = scopeActivator;
    }

    public static T CreateAndInject<T>()
    {
        if (ScopedServices is IServiceScope scope)
        {
            return ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider);
        }

        return ActivatorUtilities.CreateInstance<T>(ServiceProvider);
    }

    public static T CreateAndInject<T>(params object[] objects)
    {
        if (ScopedServices is IServiceScope scope)
        {
            return ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider, objects);
        }

        return ActivatorUtilities.CreateInstance<T>(ServiceProvider, objects);
    }

    public static async ValueTask DisposeScope()
    {
        if (ScopedServices is ScopeActivator scopeActivator)
        {
            await scopeActivator.DisposeAsync();
        }
        else if (ScopedServices is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (ScopedServices is IDisposable scopeDisposable)
        {
            scopeDisposable.Dispose();
        }
    }
}
