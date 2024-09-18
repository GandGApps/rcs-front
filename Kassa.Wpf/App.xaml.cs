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
using Kassa.Wpf.Services.PosPrinters;
using Kassa.Wpf.Services.MagneticStripeReaders;
using Kassa.Wpf.Services.CashDrawers;
using Kassa.Wpf.Controls;
using Kassa.Shared.ServiceLocator;
using Kassa.RxUI;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IEnableLogger
{

    /// <summary>
    /// I dislike always having to use <see cref="Application.Current"/> to get the current instance of <see cref="App"/>.
    /// I always need to cast, which is why I created this property.
    /// </summary>
    public static new App Current => Unsafe.As<App>(Application.Current); // using Unsafe.As is safe here because App is a singleton

    public static FontFamily LucidaConsoleFont => Current.LucidaConsoleFontFamily;

    public static object GetThemeResource(string key)
    {
        var app = Current;
        var merged = app.Resources.MergedDictionaries[0];

        return merged[key];
    }


    public FontFamily LucidaConsoleFontFamily = null!;

    public App()
    {
        var config = SharedServices.AddConfiguration("appsettings");

        if (RcsKassa.IsDevelopment)
        {
            Dispatcher.InvokeAsync(() => DeviceHelper.LogAllDevices());
        }

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        PresentationLayerServices.RegisterDispatherAdapter();

        SharedServices.AddLoggers(RcsKassa.LogsPath);

        PrinterPosLibServices.RegisterPrinterPosLib(config);
        MscReaderLibServices.RegisterMsrReaderPosLib(config);
        CashDrawerPosLibServices.RegisterCashDrawerPosLib(config);

        Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Wpf);
        //Locator.CurrentMutable.RegisterMockDataAccess(); // TODO: Replace with real data access
        //Locator.CurrentMutable.RegisterBuisnessLogic(); // TODO: Replace with real buisness logic
        BuisnessLogicServices.RegisterMockBuisnessLogic(); // TODO: Replace with real buisness logic
        DataAccessMockServices.RegisterMock(); // TODO: Replace with real data access

        HttpRepositoryServices.RegisterServices();
        BuisnessLogicEdgarServices.RegisterBuisnessLogic();

        MainViewModel.RegsiterMainViewModel();

        // Replace with view locator
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());


        typeof(SimpleRouter).TypeInitializer!.Invoke(null, null);

        ApiServiceRegistration.BuildServices();
        RcsLocatorBuilder.AddToBuilder();
        ServiceLocatorBuilder.SetLocator();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        LucidaConsoleFontFamily = (FontFamily)Resources["LucidaConsole"];
    }

    #region Init

   /* private bool _contentLoaded;

    /// <summary>
    /// InitializeComponent
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.7.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;

#line 6 "..\..\..\App.xaml"
        this.StartupUri = new System.Uri("Windows/MainWindow.xaml", System.UriKind.Relative);

#line default
#line hidden
        System.Uri resourceLocater = new System.Uri("/Kassa.Wpf;component/app.xaml", System.UriKind.Relative);

#line 1 "..\..\..\App.xaml"
        System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
    }

    /// <summary>
    /// Application Entry Point.
    /// </summary>
    [System.STAThreadAttribute()]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.7.0")]
    public static void Main()
    {
        Kassa.Wpf.App app = new Kassa.Wpf.App();
        app.InitializeComponent();
        app.Run();
    }*/

    #endregion
}

