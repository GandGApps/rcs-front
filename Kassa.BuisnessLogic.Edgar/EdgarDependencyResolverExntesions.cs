using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Refit;
using Kassa.BuisnessLogic.Edgar.Api;
using Splat.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Edgar;

public static class EdgarDependencyResolverExntesions
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


    public static void AddEdgarBuisnessLogic(this IMutableDependencyResolver services)
    {
        AddApis(services);

        services.RegisterConstant<IAuthService>(new AuthService());
    }

    internal static void AddApis(IMutableDependencyResolver services)
    {
        AddApi<ITerminalApi>(services);

        _services.AddTransient<BasicAddressHandler>();

#if DEBUG
        _services.AddTransient<HttpLoggingHandler>();
#endif

        _serviceProvider = _services.BuildServiceProvider();
    }

    internal static void AddApi<T>(IMutableDependencyResolver services, RefitSettings? settings = null) where T : class
    {
        settings ??= _refitSettings;

        _services.AddBasicApi<T>(settings);

        services.Register<T>(() => _serviceProvider.GetRequiredService<T>());
    }

    internal static IHttpClientBuilder AddBasicApi<T>(this IServiceCollection services, RefitSettings? settings = null) where T : class
    {
        return services.AddRefitClient<T>(settings)
            .AddBaseAddress()
#if DEBUG
            .AddHttpMessageHandler<HttpLoggingHandler>()
#endif
            .AddHttpMessageHandler<BasicAddressHandler>();
    }

    internal static IHttpClientBuilder AddBaseAddress(this IHttpClientBuilder builder)
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
