using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using Kassa.BuisnessLogic.Edgar;
using Kassa.BuisnessLogic;
using Kassa.DataAccess.HttpRepository;
using Kassa.DataAccess;
using Kassa.RxUI;
using Kassa.Shared.ServiceLocator;
using Kassa.Shared;
using ReactiveUI;
using Splat;
using Kassa.Avalonia.Services;
using Kassa.Avalonia.Controls;
using Kassa.Avalonia.Services.PosPrinters;
using Kassa.Avalonia.Services.MagneticStripeReaders;
using Kassa.Avalonia.Services.CashDrawers;

namespace Kassa.Avalonia;
internal static class AvaloniaPresentationLayerServiceRegistration
{
    public static AppBuilder UseRcs(this AppBuilder appBuilder)
    {
        var config = SharedServices.AddConfiguration("appsettings");

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        PresentationLayerServices.RegisterDispatherAdapter();

        SharedServices.AddLoggers(RcsKassa.LogsPath);

        PrinterPosLibServices.RegisterPrinterPosLib(config);
        MscReaderLibServices.RegisterMsrReaderPosLib(config);
        CashDrawerPosLibServices.RegisterCashDrawerPosLib(config);

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

        return appBuilder;
    }
}
