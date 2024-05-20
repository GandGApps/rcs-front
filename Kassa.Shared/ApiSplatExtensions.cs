using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Splat;

namespace Kassa.Shared;
public static class ApiSplatExtensions
{
    private static readonly ServiceCollection _services = new();
    private static readonly RefitSettings _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = LowerCaseNamingPolicy.Instance,
        }),

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

    public static void AddApi<T>(this IMutableDependencyResolver services, RefitSettings? settings = null) where T : class
    {
        settings ??= _refitSettings;

        _services.AddBasicApi<T>(settings);

        services.Register<T>(() =>
        {
            return _serviceProvider.GetRequiredService<T>();
        });
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
