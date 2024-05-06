using Kassa.Shared.DelegatingHandlers;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Splat;
using System.Text.Json;
using Kassa.DataAccess.Repositories;
using Kassa.DataAccess.Model;
using Microsoft.Extensions.Configuration;
using Kassa.DataAccess.HttpRepository.Api;

namespace Kassa.DataAccess.HttpRepository;

public static class HttpRepositoryDependencyResolverExntesions
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


    public static void AddHttpRepositoryDataAccess(this IMutableDependencyResolver services)
    {
        AddApis(services);

        services.RegisterConstant<IRepository<Member>>(new EmployeeRepository());
        services.RegisterConstant<IRepository<Order>>(new OrderRepository());
    }

    internal static void AddApis(IMutableDependencyResolver services)
    {
        AddApi<IEmployeeApi>(services);
        AddApi<IOrdersApi>(services);

        _services.AddTransient<SelectJwtDelegatingHandler>();

#if DEBUG
        _services.AddTransient<HttpDebugLoggingHandler>();
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
            .AddHttpMessageHandler<HttpDebugLoggingHandler>()
#endif
            .AddHttpMessageHandler<SelectJwtDelegatingHandler>(); 
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
