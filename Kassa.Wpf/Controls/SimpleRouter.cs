using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kassa.RxUI;
using Kassa.Shared;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Controls;
public sealed class SimpleRouter : ContentControl
{
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(SimpleRouter), new PropertyMetadata(RouterChanged));

    /// <summary>
    /// The default content property.
    /// </summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(SimpleRouter), new PropertyMetadata("not found"));


    public static readonly FrozenDictionary<Type, Func<FrameworkElement>> ViewsFor;

    static SimpleRouter()
    {
        // TODO: make generator
        var viewsFor = new Dictionary<Type, Func<FrameworkElement>>()
        {
            [typeof(Kassa.RxUI.MainViewModel)] = () => new Kassa.Wpf.MainWindow(),
            [typeof(Kassa.RxUI.AdditiveViewModel)] = () => new Kassa.Wpf.Views.AddictiveView(),
            [typeof(Kassa.RxUI.CategoryViewModel)] = () => new Kassa.Wpf.Views.CategoryView(),
            [typeof(Kassa.RxUI.ClientViewModel)] = () => new Kassa.Wpf.Views.ClientView(),
            [typeof(Kassa.RxUI.CourierViewModel)] = () => new Kassa.Wpf.Views.CourierView(),
            [typeof(Kassa.RxUI.DistrictViewModel)] = () => new Kassa.Wpf.Views.DistrictView(),
            [typeof(Kassa.RxUI.ProductViewModel)] = () => new Kassa.Wpf.Views.ProductView(),
            [typeof(Kassa.RxUI.ProductShoppingListItemViewModel)] = () => new Kassa.Wpf.Views.ShoppingListItemView(),
            [typeof(Kassa.RxUI.StreetViewModel)] = () => new Kassa.Wpf.Views.StreetView(),
            [typeof(Kassa.RxUI.SeizureReasonVm)] = () => new Kassa.Wpf.Views.SeizureReasonView(),
            [typeof(Kassa.RxUI.Pages.AllDeliveriesPageVm)] = () => new Kassa.Wpf.Pages.AllDeliveriesPage(),
            [typeof(Kassa.RxUI.Pages.AutorizationPageVm)] = () => new Kassa.Wpf.Pages.AutorizationPage(),
            [typeof(Kassa.RxUI.Pages.CashierPaymentPageVm)] = () => new Kassa.Wpf.Pages.CashierPaymentPage(),
            [typeof(Kassa.RxUI.Pages.CashierShiftReportPageVm)] = () => new Kassa.Wpf.Pages.CashierShiftReportPage(),
            [typeof(Kassa.RxUI.Pages.DeliveryOrderEditPageVm)] = () => new Kassa.Wpf.Pages.DeliveryOrderEditPage(),
            [typeof(Kassa.RxUI.Pages.DeliveryPaymentPageVm)] = () => new Kassa.Wpf.Pages.DeliveryPaymentPage(),
            [typeof(Kassa.RxUI.Pages.EditDeliveryPageVm)] = () => new Kassa.Wpf.Pages.EditDeliveryPage(),
            [typeof(Kassa.RxUI.Pages.MainPageVm)] = () => new Kassa.Wpf.Pages.MainPage(),
            [typeof(Kassa.RxUI.Pages.NewDeliveryPageVm)] = () => new Kassa.Wpf.Pages.NewDeliveryPage(),
            [typeof(Kassa.RxUI.Pages.OrderEditPageVm)] = () => new Kassa.Wpf.Pages.OrderEditPage(),
            [typeof(Kassa.RxUI.Pages.PersonalPageVm)] = () => new Kassa.Wpf.Pages.PersonalPage(),
            [typeof(Kassa.RxUI.Pages.PincodePageVm)] = () => new Kassa.Wpf.Pages.PincodePage(),
            [typeof(Kassa.RxUI.Pages.ServicePageVm)] = () => new Kassa.Wpf.Pages.ServicePage(),
            [typeof(Kassa.RxUI.Pages.OrderEditWithNavigationPageVm)] = () => new Kassa.Wpf.Pages.OrderEditWithNavigationPage(),
            [typeof(Kassa.RxUI.Pages.OrderEditWithNavigationPageItemVm)] = () => new Kassa.Wpf.Pages.OrderEditWithNavigationPageItem(),
            [typeof(Kassa.RxUI.Dialogs.AllClientsDialogViewModel)] = () => new Kassa.Wpf.Dialogs.AllClientsDialog(),
            [typeof(Kassa.RxUI.Dialogs.AllDistrictsDialogViewModel)] = () => new Kassa.Wpf.Dialogs.AllDistrictsDialog(),
            [typeof(Kassa.RxUI.Dialogs.AreYouSureToTurnOffDialogViewModel)] = () => new Kassa.Wpf.Dialogs.AreYouSureToTurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.CommentDialogViewModel)] = () => new Kassa.Wpf.Dialogs.CommentDialog(),
            [typeof(Kassa.RxUI.Dialogs.DeliviryDialogViewModel)] = () => new Kassa.Wpf.Dialogs.DeliviryDialog(),
            [typeof(Kassa.RxUI.Dialogs.DiscountsAndSurchargesDialogViewModel)] = () => new Kassa.Wpf.Dialogs.DiscountsAndSurchargesDialog(),
            [typeof(Kassa.RxUI.Dialogs.DocumentsDialogViewModel)] = () => new Kassa.Wpf.Dialogs.DocumentsDialog(),
            [typeof(Kassa.RxUI.Dialogs.EmaiEditlDialogViewModel)] = () => new Kassa.Wpf.Dialogs.EmailEditDialog(),
            [typeof(Kassa.RxUI.Dialogs.EnterPincodeDialogViewModel)] = () => new Kassa.Wpf.Dialogs.EnterPincodeDialog(),
            [typeof(Kassa.RxUI.Dialogs.HintDialogViewModel)] = () => new Kassa.Wpf.Dialogs.HintDialog(),
            [typeof(Kassa.RxUI.Dialogs.InputDialogViewModel)] = () => new Kassa.Wpf.Dialogs.InputDialog(),
            [typeof(Kassa.RxUI.Dialogs.InputNumberDialogViewModel)] = () => new Kassa.Wpf.Dialogs.InputNumberDialog(),
            [typeof(Kassa.RxUI.Dialogs.LoadingDialogViewModel)] = () => new Kassa.Wpf.Dialogs.LoadingDialog(),
            [typeof(Kassa.RxUI.Dialogs.MoreCashierDialogViewModel)] = () => new Kassa.Wpf.Dialogs.MoreCashierDialog(),
            [typeof(Kassa.RxUI.Dialogs.OkMessageDialogViewModel)] = () => new Kassa.Wpf.Dialogs.OkMessageDialog(),
            [typeof(Kassa.RxUI.Dialogs.PersonnelDialogViewModel)] = () => new Kassa.Wpf.Dialogs.PersonnelDialog(),
            [typeof(Kassa.RxUI.Dialogs.PincodeTurnOffDialogViewModel)] = () => new Kassa.Wpf.Dialogs.PincodeTurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.PortionDialogVm)] = () => new Kassa.Wpf.Dialogs.PortionDialog(),
            [typeof(Kassa.RxUI.Dialogs.ProblemDialogViewModel)] = () => new Kassa.Wpf.Dialogs.ProblemDialog(),
            [typeof(Kassa.RxUI.Dialogs.ProfileDialogViewModel)] = () => new Kassa.Wpf.Dialogs.ProfileDialog(),
            [typeof(Kassa.RxUI.Dialogs.PromocodeDialogViewModel)] = () => new Kassa.Wpf.Dialogs.PromocodeDialog(),
            [typeof(Kassa.RxUI.Dialogs.QuantityVolumeDialogVewModel)] = () => new Kassa.Wpf.Dialogs.QuantityVolumeDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchAddictiveDialogViewModel)] = () => new Kassa.Wpf.Dialogs.SearchAddictiveDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchCourierDialogViewModel)] = () => new Kassa.Wpf.Dialogs.SearchCourierDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchProductDialogViewModel)] = () => new Kassa.Wpf.Dialogs.SearchProductDialog(),
            [typeof(Kassa.RxUI.Dialogs.SendReceiptDialogViewModel)] = () => new Kassa.Wpf.Dialogs.SendReceiptDialog(),
            [typeof(Kassa.RxUI.Dialogs.ServicesDialogViewModel)] = () => new Kassa.Wpf.Dialogs.ServicesDialog(),
            [typeof(Kassa.RxUI.Dialogs.StreetsDialogViewModel)] = () => new Kassa.Wpf.Dialogs.StreetsDialog(),
            [typeof(Kassa.RxUI.Dialogs.TurnOffDialogViewModel)] = () => new Kassa.Wpf.Dialogs.TurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.SeizureReasonDialogViewModel)] = () => new Kassa.Wpf.Dialogs.SeizureReasonDialog(),
            [typeof(Kassa.RxUI.Dialogs.MemberSelectDialogViewModel)] = () => new Kassa.Wpf.Dialogs.MemberSelectDialog(),
            [typeof(Kassa.RxUI.Dialogs.FundActDialogViewModel)] = () => new Kassa.Wpf.Dialogs.FundActDialog(),
            [typeof(Kassa.RxUI.Dialogs.ContributionReasonDialogViewModel)] = () => new Kassa.Wpf.Dialogs.ContributionReasonDialog(),
        };

        ViewsFor = viewsFor.ToFrozenDictionary();

        /*foreach (var ti in Assembly.GetCallingAssembly().DefinedTypes
                                   .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
        {
            // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
            var ivf = ti.ImplementedInterfaces.FirstOrDefault(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

            // need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
            if (ivf is not null)
            {
                LogHost.Default.Debug($"Registering {ti.FullName} for {ivf.FullName}");
            }
        }*/
    }

    public static void RouterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimpleRouter router)
        {
            router._disposables.Clear();

            router._disposables.Add(router.Router.CurrentViewModel.Subscribe(vm =>
            {
                if (vm is null)
                {
                    router.Content = router.DefaultContent;
                    return;
                }

                if (ViewsFor.TryGetValue(vm.GetType(), out var viewFactory))
                {
                    var view = viewFactory();
                    view.DataContext = vm;

                    if (view is IViewFor viewFor)
                    {
                        viewFor.ViewModel = vm;
                    }

                    router.Content = view;
                }
                else
                {
                    router.Content = router.DefaultContent;
                    LogHost.Default.Warn($"View for {vm.GetType().FullName} not found");
                }
            }));
        }
    }

    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    private readonly CompositeDisposable _disposables = [];

    public SimpleRouter()
    {
        Unloaded += (_, _) => _disposables.Dispose();
    }
}
