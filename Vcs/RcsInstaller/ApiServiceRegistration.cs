using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RcsInstaller.DelegatingHandlers;
using RcsVersionControlMock.Json;
using Refit;
using Splat;
using System;

namespace RcsInstaller;
public static class ApiServiceRegistration
{
    private static readonly RefitSettings _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(RcsVCConstants.JsonSerializerOptions),

    };


    public static void AddApi<T>(this IServiceCollection services, RefitSettings? settings = null) where T : class
    {
        settings ??= _refitSettings;

        services.TryAddTransient<SelectJwtDelegatingHandler>();
        services.TryAddTransient<HttpDebugLoggingHandler>();

        services.AddBasicApi<T>(settings);
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
        return builder.ConfigureHttpClient((serviceProvider, httpClient) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var baseAddress = configuration["ApiConfiguration:BaseAddress"]!;

            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                ThrowHelper.ThrowInvalidOperationException("ApiConfiguration:BaseAddress is not set");
            }

            httpClient.BaseAddress = new Uri(baseAddress);
        });
    }
}