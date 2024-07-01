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

    public static void SetThemeResource(string key, object value)
    {
        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];

        merged[key] = value;
    }

    public static object GetThemeResource(string key)
    {

        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];

        return merged[key];
    }

    public static void SwitchTheme(Theme theme) => SwitchTheme(theme.ToString());

    public static void SwitchTheme(string themeName)
    {
        var app = (App)Current;

        var theme = new ResourceDictionary
        {
            Source = new Uri($"Themes/{themeName}Generated.xaml", UriKind.Relative)
        };

        app.Resources.MergedDictionaries.Insert(0,theme);
        app.Resources.MergedDictionaries.RemoveAt(1);
    }


    /// <summary>
    /// Get current theme name
    /// </summary>
    /// <returns>$"Themes/{themeName}Generated.xaml" theme name</returns>
    public static Theme GetCurrentThemeName()
    {
        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];

        var themeName = merged.Source.OriginalString.Split('/').LastOrDefault()?.Split('.').FirstOrDefault();

        if (string.IsNullOrEmpty(themeName))
            throw new Exception("Theme name is null or empty");

        themeName = themeName[..^"Generated".Length];

        return Enum.Parse<Theme>(themeName);
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

        Locator.CurrentMutable.RegisterConstant<IPrinter>(new Printer());

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

