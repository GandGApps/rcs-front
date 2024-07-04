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

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static readonly CultureInfo RuCulture = new("ru-RU");

    public static object GetThemeResource(string key)
    {

        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];

        return merged[key];
    }

    public App()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = configurationBuilder.Build();

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        Locator.CurrentMutable.AddLoggers();

        Locator.CurrentMutable.RegisterConstant<IConfiguration>(config);

        var posLibString = config.GetValue<string>("PosLib");

        var poslib = Enum.TryParse<PosLib>(posLibString, true, out var pos) ? pos : PosLib.Wndpos;

        switch (poslib)
        {
            case PosLib.Wndpos:
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new WndPosPrinter());
                break;
            case PosLib.Mcpos:
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new McPrinter());
                break;
            case PosLib.Escpos:
                var port = config.GetValue<string>("EscposPrinterPort");
                Locator.CurrentMutable.RegisterConstant<IPrinter>(new EscPosPrinter(port));
                break;
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
    }
}

