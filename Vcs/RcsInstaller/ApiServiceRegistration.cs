using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RcsInstaller.DelegatingHandlers;
using RcsVersionControlMock.Json;
using Refit;
using Splat;
using System;

namespace RcsInstaller;
public static class ApiServiceRegistration
{
    private static readonly ServiceCollection _services = new();
    private static readonly RefitSettings _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(RcsVCConstants.JsonSerializerOptions),

    };

    private static IServiceProvider _serviceProvider = null!;

    /// <summary>
    /// Call this method at the end
    /// </summary>
    public static void BuildServices()
    {
        _services.AddTransient<SelectJwtDelegatingHandler>();
        _services.AddTransient<HttpDebugLoggingHandler>();

        _serviceProvider = _services.BuildServiceProvider();
    }

    public static void AddApi<T>(RefitSettings? settings = null) where T : class
    {
        settings ??= _refitSettings;

        _services.AddBasicApi<T>(settings);

        Locator.CurrentMutable.Register<T>(() => _serviceProvider.GetRequiredService<T>());
    }

    public static T GetService<T>() where T : class
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public static IHttpClientBuilder AddBasicApi<T>(this IServiceCollection services, RefitSettings? settings = null) where T : class
    {
        return services.AddRefitClient<T>(settings)
            .AddBaseAddress()
            .AddHttpMessageHandler<SelectJwtDelegatingHandler>()
            .AddHttpMessageHandler<HttpDebugLoggingHandler>();
    }

    public static IHttpClientBuilder AddBaseAddress(this IHttpClientBuilder builder)
    {
        return builder.ConfigureHttpClient(httpClient =>
        {
            var configuration = Locator.Current.GetRequiredService<IConfiguration>();

            var baseAddress = configuration["ApiConfiguration:BaseAddress"]!;

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new InvalidOperationException("Base address is not set");
            }

            httpClient.BaseAddress = new Uri(baseAddress);
        });
    }
}