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
        var config = Locator.CurrentMutable.AddConfiguration("appsettings.json");

        EnvironmentName = config.GetValue<string>("Environment") ?? "Production";

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        Locator.CurrentMutable.AddLoggers(LogsPath);

        var posLibString = config.GetValue<string>(nameof(PrinterPosLib));

        var poslib = Enum.TryParse<PrinterPosLib>(posLibString, true, out var pos) ? pos : PrinterPosLib.Wndpos;

        this.Log().Info($"Selected poslib: {poslib}");

        var magneticStripeReader = new WndPosMagneticStripeReader();

        Dispatcher.InvokeAsync(async () => await magneticStripeReader.TryClaim());

        switch (poslib)
        {
            case PrinterPosLib.Wndpos:
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new WndPosPrinter());
                break;
            case PrinterPosLib.Escpos:
                var port = config.GetValue<string>("EscposPrinterPort");
                if (string.IsNullOrWhiteSpace(port))
                {
                    this.Log().Error("Port for Escpos printer is not set");
                    break;
                }
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new EscPosPrinter(port));
                break;
            case PrinterPosLib.Wnd:
                var useDefaultPrinter = config.GetValue<bool>("UseDefaultPrinter");
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new WndPrinter(useDefaultPrinter));
                break;
            // TODO: Fix or remove this implementation
            /*case PrinterPosLib.EscposUsb:
                var printerName = config.GetValue<string>("EscposUsbPrinterName");
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new EscPosUsbPrinter(printerName));
                break;*/
            default:
                break;
        }

        Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Wpf);
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterMockDataAccess(); // TODO: Replace with real data access
        Locator.CurrentMutable.RegisterMockBuisnessLogic(); // TODO: Replace with real buisness logic

        Locator.CurrentMutable.AddHttpRepositoryDataAccess();
        Locator.CurrentMutable.AddEdgarBuisnessLogic();

        ApiSplatExtensions.BuildServices();

        RxApp.SuppressViewCommandBindingMessage = true;
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        LucidaConsoleFontFamily = (FontFamily)Resources["LucidaConsole"];
    }

}

