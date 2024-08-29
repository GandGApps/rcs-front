using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;

namespace Kassa.Shared;
public static class RcsKassa
{
    public const string ProductionName = "Production";
    public const string DevelopmentName = "Development";

    public static readonly CultureInfo RuCulture = new("ru-RU");

    public static string EnvironmentName
    {
        get
        {
            var config = RcsLocator.GetRequiredService<IConfiguration>();

            return config.GetValue<string>("Environment") ?? ProductionName;
        }
    } 

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static string LogsPath => Path.Combine(BasePath, "logs", "Logs.txt");

    public static bool IsDevelopment => string.Equals(EnvironmentName, DevelopmentName, StringComparison.InvariantCultureIgnoreCase);
    public static bool IsProduction => string.Equals(EnvironmentName, ProductionName, StringComparison.InvariantCultureIgnoreCase);
}
