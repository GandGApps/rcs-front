using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Splat;

namespace Kassa.Avalonia.Controls;
public sealed class SimpleRouter: ContentControl
{
    public static readonly StyledProperty<RoutingState> RouterProperty =
        AvaloniaProperty.Register<SimpleRouter, RoutingState>(nameof(Router));

    public static readonly StyledProperty<object> DefaultContentProperty =
        AvaloniaProperty.Register<SimpleRouter, object>(nameof(DefaultContent), "not found");

    public static readonly FrozenDictionary<Type, Func<StyledElement>> ViewsFor;

    static SimpleRouter()
    {
        RouterProperty.Changed.AddClassHandler<SimpleRouter>(RouterChanged);

        // TODO: make generator
        /*var viewsFor = new Dictionary<Type, Func<StyledElement>>()
        {
            [typeof(Kassa.RxUI.MainViewModel)] = () => new Kassa.Avalonia.MainWindow(),
            [typeof(Kassa.RxUI.AdditiveViewModel)] = () => new Kassa.Avalonia.Views.AddictiveView(),
            [typeof(Kassa.RxUI.CategoryViewModel)] = () => new Kassa.Avalonia.Views.CategoryView(),
            [typeof(Kassa.RxUI.ClientViewModel)] = () => new Kassa.Avalonia.Views.ClientView(),
            [typeof(Kassa.RxUI.CourierViewModel)] = () => new Kassa.Avalonia.Views.CourierView(),
            [typeof(Kassa.RxUI.DistrictViewModel)] = () => new Kassa.Avalonia.Views.DistrictView(),
            [typeof(Kassa.RxUI.ProductViewModel)] = () => new Kassa.Avalonia.Views.ProductView(),
            [typeof(Kassa.RxUI.ProductShoppingListItemViewModel)] = () => new Kassa.Avalonia.Views.ShoppingListItemView(),
            [typeof(Kassa.RxUI.StreetViewModel)] = () => new Kassa.Avalonia.Views.StreetView(),
            [typeof(Kassa.RxUI.SeizureReasonVm)] = () => new Kassa.Avalonia.Views.SeizureReasonView(),
            [typeof(Kassa.RxUI.Pages.AllDeliveriesPageVm)] = () => new Kassa.Avalonia.Pages.AllDeliveriesPage(),
            [typeof(Kassa.RxUI.Pages.AutorizationPageVm)] = () => new Kassa.Avalonia.Pages.AutorizationPage(),
            [typeof(Kassa.RxUI.Pages.CashierPaymentPageVm)] = () => new Kassa.Avalonia.Pages.CashierPaymentPage(),
            [typeof(Kassa.RxUI.Pages.CashierShiftReportPageVm)] = () => new Kassa.Avalonia.Pages.CashierShiftReportPage(),
            [typeof(Kassa.RxUI.Pages.DeliveryOrderEditPageVm)] = () => new Kassa.Avalonia.Pages.DeliveryOrderEditPage(),
            [typeof(Kassa.RxUI.Pages.DeliveryPaymentPageVm)] = () => new Kassa.Avalonia.Pages.DeliveryPaymentPage(),
            [typeof(Kassa.RxUI.Pages.EditDeliveryPageVm)] = () => new Kassa.Avalonia.Pages.EditDeliveryPage(),
            [typeof(Kassa.RxUI.Pages.MainPageVm)] = () => new Kassa.Avalonia.Pages.MainPage(),
            [typeof(Kassa.RxUI.Pages.NewDeliveryPageVm)] = () => new Kassa.Avalonia.Pages.NewDeliveryPage(),
            [typeof(Kassa.RxUI.Pages.OrderEditPageVm)] = () => new Kassa.Avalonia.Pages.OrderEditPage(),
            [typeof(Kassa.RxUI.Pages.PersonalPageVm)] = () => new Kassa.Avalonia.Pages.PersonalPage(),
            [typeof(Kassa.RxUI.Pages.PincodePageVm)] = () => new Kassa.Avalonia.Pages.PincodePage(),
            [typeof(Kassa.RxUI.Pages.ServicePageVm)] = () => new Kassa.Avalonia.Pages.ServicePage(),
            [typeof(Kassa.RxUI.Dialogs.AllClientsDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.AllClientsDialog(),
            [typeof(Kassa.RxUI.Dialogs.AllDistrictsDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.AllDistrictsDialog(),
            [typeof(Kassa.RxUI.Dialogs.AreYouSureToTurnOffDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.AreYouSureToTurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.CommentDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.CommentDialog(),
            [typeof(Kassa.RxUI.Dialogs.DeliviryDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.DeliviryDialog(),
            [typeof(Kassa.RxUI.Dialogs.DiscountsAndSurchargesDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.DiscountsAndSurchargesDialog(),
            [typeof(Kassa.RxUI.Dialogs.DocumentsDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.DocumentsDialog(),
            [typeof(Kassa.RxUI.Dialogs.EmaiEditlDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.EmailEditDialog(),
            [typeof(Kassa.RxUI.Dialogs.EnterPincodeDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.EnterPincodeDialog(),
            [typeof(Kassa.RxUI.Dialogs.HintDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.HintDialog(),
            [typeof(Kassa.RxUI.Dialogs.InputDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.InputDialog(),
            [typeof(Kassa.RxUI.Dialogs.InputNumberDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.InputNumberDialog(),
            [typeof(Kassa.RxUI.Dialogs.LoadingDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.LoadingDialog(),
            [typeof(Kassa.RxUI.Dialogs.MoreCashierDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.MoreCashierDialog(),
            [typeof(Kassa.RxUI.Dialogs.OkMessageDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.OkMessageDialog(),
            [typeof(Kassa.RxUI.Dialogs.PersonnelDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.PersonnelDialog(),
            [typeof(Kassa.RxUI.Dialogs.PincodeTurnOffDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.PincodeTurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.PortionDialogVm)] = () => new Kassa.Avalonia.Dialogs.PortionDialog(),
            [typeof(Kassa.RxUI.Dialogs.ProblemDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.ProblemDialog(),
            [typeof(Kassa.RxUI.Dialogs.ProfileDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.ProfileDialog(),
            [typeof(Kassa.RxUI.Dialogs.PromocodeDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.PromocodeDialog(),
            [typeof(Kassa.RxUI.Dialogs.QuantityVolumeDialogVewModel)] = () => new Kassa.Avalonia.Dialogs.QuantityVolumeDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchAddictiveDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.SearchAddictiveDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchCourierDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.SearchCourierDialog(),
            [typeof(Kassa.RxUI.Dialogs.SearchProductDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.SearchProductDialog(),
            [typeof(Kassa.RxUI.Dialogs.SendReceiptDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.SendReceiptDialog(),
            [typeof(Kassa.RxUI.Dialogs.ServicesDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.ServicesDialog(),
            [typeof(Kassa.RxUI.Dialogs.StreetsDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.StreetsDialog(),
            [typeof(Kassa.RxUI.Dialogs.TurnOffDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.TurnOffDialog(),
            [typeof(Kassa.RxUI.Dialogs.SeizureReasonDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.SeizureReasonDialog(),
            [typeof(Kassa.RxUI.Dialogs.MemberSelectDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.MemberSelectDialog(),
            [typeof(Kassa.RxUI.Dialogs.FundActDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.FundActDialog(),
            [typeof(Kassa.RxUI.Dialogs.ContributionReasonDialogViewModel)] = () => new Kassa.Avalonia.Dialogs.ContributionReasonDialog(),
        };*/

        //ViewsFor = viewsFor.ToFrozenDictionary();
    }

    public static void RouterChanged(SimpleRouter d, AvaloniaPropertyChangedEventArgs e)
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
        get => GetValue(RouterProperty);
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
