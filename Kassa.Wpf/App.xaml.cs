using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using ReactiveUI;
using Splat;
using Kassa.BuisnessLogic;
using Kassa.DataAccess;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly Dictionary<string, string[]> _lightTheme = new()
{
    { "Theme", new[] { "SubtotalButtonBorderBrush", "SubtotalSeparator" } },
    { "SecondaryBackground", new[] { "DefaultAdditiveViewBackground", "PricingDetailsButtonBackground", "ProductHostBackground", "MultiSelectCheckBoxCheckedBackground" } },
    { "SurfaceBrush", new[] { "DefaultProductViewIconFill", "NotAvailableProductViewIconFill", "SelecteProductViewIconFill", "DefaultAdditiveViewIconFill", "NotAvailbaleAdditiveViewIconFill", "SelectedAdditiveViewIconFill", "TopBarIconFill", "CategoryViewIconFill" } },
    { "DisabledBrush", new[] { "DisabledButtonBackground" } },
    { "AccentBrush", new[] { "DefaultButtonBackground", "DialogButtonHover" } },
    { "AccentBrush2", new[] { "SelectedProductViewBackground", "SelecteProductViewPriceBackground", "SelectedAdditiveViewBackground", "AccentTabPanel" } },
    { "AccentBrush3", []},
    { "SecondaryBrush", new[] { "DefaultProductViewBackground", "DefaultCardBackground", "DialogHeaderBackground", "KeyBackground", "CategoryViewBackground" } },
    { "PrimaryForeground", new[] { "DefaultProductViewForeground", "NotAvailableProductProductViewForeground", "SelectedProductViewForeground", "DefaultAdditiveViewForeground", "NotAvailbaleAdditiveViewForeground", "SelectedAdditiveViewForeground", "PricingDetailsButtonForeground", "ProductHostForeground" } },
    { "AlternateForeground", new[] { "DefaultButtonForeground", "IconOnlyButtonIconFill", "MultiSelectCheckBoxForeground", "WorkingShiftForeground", "ReceiptNumberForeground", "UserForeground", "DownMenuBackButtonForeground", "DownMenuSelectedBrush", "DownMenuIconFillBrush", "TabPanelIconFill", "DefaultMainButton" } },
    { "SecondaryForeground", new[] { "DefaultProdcutViewPriceForeground", "SelecteProdcutViewPriceForeground", "DefaultAdditveViewMeasureForeground", "NotAvailbaleAdditveViewMeasureForeground", "SelectedAdditveViewMeasureForeground", "TopBarForegeround", "KeyForeground", "DialogSubheaderForeground" } },
    { "DangerBrush", new[] { "NotAvailableProdcutViewPriceForeground", "NotAvailableTextProductView", "NotAvailbaleTextAdditveView", "DialogTurnOffDevice" } },
    { "DangerBrushAlpha", new[] { "NotAvailableProductViewBackground", "NotAvailableProductViewPriceBackground", "NotAvailbaleAdditiveViewBackground" } },
    { "SuccessBrush", new[] { "DialogOptionsIconFill" } },
    { "TopShadowBrush", [] },
    { "BottomShadowBrush", [] },
    { "AccentBorderBrush", [] },
    { "BlurColor", [] },
};

    public static void SetSolidBrush(string brushKey, Color color)
    {
        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];

        foreach (var key in _lightTheme[brushKey])
        {
            merged[key] = new SolidColorBrush(color);
        }

        merged[brushKey] = new SolidColorBrush(color);
    }
    public App()
    {
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterMockDataAccess(); // TODO: Replace with real data access
        Locator.CurrentMutable.RegisterBuisnessLogic();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
    }
}

