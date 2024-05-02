using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Refit;
using Kassa.BuisnessLogic.Edgar.Api;
using Splat.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kassa.BuisnessLogic.Edgar;

public static class EdgarDependencyResolverExntesions
{
    private static readonly ServiceCollection _services = new();

    private static IServiceProvider _serviceProvider = null!;

    public static void AddEdgarBuisnessLogic(this IMutableDependencyResolver services)
    {
        AddApis();

        services.RegisterConstant<IAuthService>(new AuthService());
    }

    internal static void AddApis()
    {
        _services.AddRefitClient<ITerminalApi>()
            .AddBaseAddress();

        _services.UseMicrosoftDependencyResolver();

        _serviceProvider = _services.BuildServiceProvider();
        _serviceProvider.UseMicrosoftDependencyResolver();
    }

    internal static IHttpClientBuilder AddBasicApi<T>(this IServiceCollection services, RefitSettings? settings = null) where T : class
    {
        return services.AddRefitClient<T>(settings)
            .AddBaseAddress()
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
