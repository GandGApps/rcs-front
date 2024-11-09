using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Splat;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CommunityToolkit.Diagnostics;

namespace Kassa.Shared;
public static class ApiServiceRegistration
{
    private static readonly RefitSettings _refitSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = LowerCaseNamingPolicy.Instance,
            Converters = { new DefaultIfNullConverter<int>(), new DefaultIfNullConverter<double>(), new DefaultIfNullConverter<bool>() }
        }),

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
                ThrowHelper.ThrowInvalidOperationException("Base address is not set");
            }

            httpClient.BaseAddress = new Uri(baseAddress);
        });
    }
}
