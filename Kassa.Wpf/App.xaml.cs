using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Edgar;
using Kassa.DataAccess.HttpRepository;
using Kassa.DataAccess;
using Kassa.Shared;
using Kassa.Wpf.Themes;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;
using Kassa.BuisnessLogic.Services;
using Kassa.Wpf.Services;
using System.IO;
using System.Runtime.CompilerServices;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IEnableLogger
{
    public static readonly CultureInfo RuCulture = new("ru-RU");

    /// <summary>
    /// I dislike always having to use <see cref="Application.Current"/> to get the current instance of <see cref="App"/>.
    /// I always need to cast, which is why I created this property.
    /// </summary>
    public static new App Current => Unsafe.As<App>(Application.Current); // using Unsafe.As is safe here because App is a singleton

    public static FontFamily LucidaConsoleFont => Current.LucidaConsoleFontFamily;

    public static bool IsDevelopment => string.Equals(EnvironmentName, "Development", StringComparison.InvariantCultureIgnoreCase); // using Unsafe.As is safe here because App is a singleton

    public static bool IsProduction => string.Equals(EnvironmentName, "Production", StringComparison.InvariantCultureIgnoreCase); // using Unsafe.As is safe here because App is a singleton

    public static string EnvironmentName
    {
        get; private set;
    } = null!;

    public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

    public static string LogsPath => Path.Combine(BasePath, "logs", "Logs.txt");

    public static object GetThemeResource(string key)
    {
        var app = Current;
        var merged = app.Resources.MergedDictionaries[0];

        return merged[key];
    }


    public FontFamily LucidaConsoleFontFamily = null!;

    public App()
    {
        var config = Locator.CurrentMutable.AddConfiguration("appsettings");

        EnvironmentName = config.GetValue<string>("Environment") ?? "Production";

        if (EnvironmentName == "Development")
        {
            Dispatcher.InvokeAsync(() => DeviceHelper.LogAllDevices());
        }

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        Locator.CurrentMutable.AddLoggers(LogsPath);

        Locator.CurrentMutable.AddPrinterPosLib(config);
        Locator.CurrentMutable.AddMsrReaderPosLib(config);
        Locator.CurrentMutable.AddCashDrawerPosLib(config);

        Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Wpf);
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterMockDataAccess(); // TODO: Replace with real data access
        Locator.CurrentMutable.RegisterMockBuisnessLogic(); // TODO: Replace with real buisness logic

        Locator.CurrentMutable.AddHttpRepositoryDataAccess();
        Locator.CurrentMutable.AddEdgarBuisnessLogic();

        ApiSplatExtensions.BuildServices();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        LucidaConsoleFontFamily = (FontFamily)Resources["LucidaConsole"];
    }

}

