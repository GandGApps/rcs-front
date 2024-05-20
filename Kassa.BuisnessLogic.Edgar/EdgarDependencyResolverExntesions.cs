using Kassa.BuisnessLogic.Edgar.Services;
using Kassa.BuisnessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Refit;
using Kassa.BuisnessLogic.Edgar.Api;
using Splat.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Kassa.Shared.DelegatingHandlers;
using Kassa.Shared;
using Kassa.DataAccess.Repositories;
using Kassa.DataAccess.Model;

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

        services.Register<IShiftService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Shift>>();
            var memberService = Locator.Current.GetNotInitializedService<IMemberService>();

            return new ShiftService(repository, memberService);
        });
        services.RegisterInitializableServiceFactory<IShiftService>();

        services.Register<IProductService>(() =>
        {
            var repository = Locator.Current.GetRequiredService<IRepository<Product>>();
            var ingridientsService = Locator.Current.GetRequiredService<IIngridientsService>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();

            return new ProductService(repository, ingridientsService, receiptService);
        });

        services.Register<ICashierService>(() =>
        {
            var additiveService = Locator.Current.GetNotInitializedService<IAdditiveService>();
            var categoryService = Locator.Current.GetNotInitializedService<ICategoryService>();
            var productService = Locator.Current.GetNotInitializedService<IProductService>();
            var receiptService = Locator.Current.GetNotInitializedService<IReceiptService>();
            var ordersService = Locator.Current.GetNotInitializedService<IOrdersService>();
            var paymentInfoService = Locator.Current.GetNotInitializedService<IPaymentInfoService>();

            return new CashierService(additiveService, categoryService, productService, receiptService, ordersService, paymentInfoService);
        });
        services.RegisterInitializableServiceFactory<ICashierService>();

        SplatRegistrations.SetupIOC();
    }

    internal static void AddApis(IMutableDependencyResolver services)
    {
        AddApi<ITerminalApi>(services);
        AddApi<IEmployeePostsApi>(services);
        AddApi<IEmployeeApi>(services);

        _services.AddTransient<SelectJwtDelegatingHandler>();
        _services.AddTransient<HttpDebugLoggingHandler>();

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
            .AddHttpMessageHandler<SelectJwtDelegatingHandler>()
            .AddHttpMessageHandler<HttpDebugLoggingHandler>()
            ;
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
