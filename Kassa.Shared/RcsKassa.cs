using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;

namespace Kassa.Shared;
public static class RcsKassa
{
    public const string ProductionName = "Production";
    public const string DevelopmentName = "Development";

    private static IConfiguration? _configuration;
    public static IConfiguration Configuration
    {
        get => _configuration ??= RcsLocator.GetRequiredService<IConfiguration>();
        set => _configuration = value;

    }

    public static readonly CultureInfo RuCulture = new("ru-RU");

    public static string EnvironmentName => Configuration.GetValue<string>("Environment") ?? ProductionName;

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static string LogsPath => Path.Combine(BasePath, "logs", "Logs.txt");

    public static bool IsDevelopment => string.Equals(EnvironmentName, DevelopmentName, StringComparison.InvariantCultureIgnoreCase);
    public static bool IsProduction => string.Equals(EnvironmentName, ProductionName, StringComparison.InvariantCultureIgnoreCase);

    public static JsonSerializerOptions JsonSerializerOptions
    {
        get; private set;
    } = new()
    {
        TypeInfoResolver = null,
        Converters = {
            new DefaultIfNullConverter<int>(), new DefaultIfNullConverter<double>(), new DefaultIfNullConverter<bool>()
        }
    };

    public static void AddContext(JsonSerializerContext context)
    {
        if (JsonSerializerOptions.TypeInfoResolverChain.Contains(context))
        {
            return;
        }

        if (JsonSerializerOptions.IsReadOnly)
        {
            JsonSerializerOptions = new(JsonSerializerOptions);
        }

        JsonSerializerOptions.TypeInfoResolverChain.Add(context);
    }
}
