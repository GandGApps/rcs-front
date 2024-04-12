using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class AllDeliveriesPageVm : PageViewModel
{
    public AllDeliveriesPageVm()
    {
        Date = DateTime.UtcNow;

        MoveDateCommand = ReactiveCommand.Create<int>(x =>
        {
            Date = Date.AddDays(x);
        });

        GoToPickUpCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var clientService = await Locator.GetInitializedService<IClientService>();

            var allClientsDialogViewModel = new AllClientsDialogViewModel(clientService)
            {
                IsPickup = true
            };

            await MainViewModel.DialogOpenCommand.Execute(allClientsDialogViewModel).FirstAsync();
        }).DisposeWith(InternalDisposables);

        GoToDeliveryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var clientService = await Locator.GetInitializedService<IClientService>();

            var allClientsDialogViewModel = new AllClientsDialogViewModel(clientService)
            {
                IsDelivery = true
            };

            await MainViewModel.DialogOpenCommand.Execute(allClientsDialogViewModel).FirstAsync();
        }).DisposeWith(InternalDisposables);

        EditOrderCommand = ReactiveCommand.CreateFromTask<OrderDto>(async order =>
        {
            var cashierService = await Locator.GetInitializedService<ICashierService>();
            var additiveService = await Locator.GetInitializedService<IAdditiveService>();
            var clientService = await Locator.GetInitializedService<IClientService>();
            var courierService = await Locator.GetInitializedService<ICourierService>();
            var districtService = await Locator.GetInitializedService<IDistrictService>();
            var streetService = await Locator.GetInitializedService<IStreetService>();

            var client = order.ClientId.HasValue ? await clientService.GetClientById(order.ClientId.Value) : null;
            var courier = order.CourierId.HasValue ? await courierService.GetCourierById(order.CourierId.Value) : null;
            var district = order.DistrictId.HasValue ? await districtService.GetDistrictById(order.DistrictId.Value) : null;
            var street = order.StreetId.HasValue ? await streetService.GetStreetById(order.StreetId.Value) : null;

            var editDeliveryPageVm = new EditDeliveryPageVm(cashierService, additiveService, client, courier, order, district, street);

            MainViewModel.GoToPageCommand.Execute(editDeliveryPageVm).Subscribe();

        }).DisposeWith(InternalDisposables);
    }

    public ReactiveCommand<Unit, Unit> GoToPickUpCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoToDeliveryCommand
    {
        get;
    }

    [Reactive]
    public bool IsPickup
    {
        get; set;
    }

    [Reactive]
    public bool IsDelivery
    {
        get; set;
    }

    [Reactive]
    public DateTime Date
    {
        get; set;
    }

    public ReactiveCommand<int, Unit> MoveDateCommand
    {
        get;
    }

    public extern int InUncomfiredOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int NewOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int InCookingOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int CompletedOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int CanceledOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int ReadyOrders
    {
        [ObservableAsProperty]
        get;
    }

    public extern int OnTheWayOrders
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public ReadOnlyObservableCollection<OrderRowViewModel>? Orders
    {
        get; set;
    }

    [Reactive]
    public string SearchedText
    {
        get; set;
    } = string.Empty;

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    public ReactiveCommand<OrderDto, Unit> EditOrderCommand
    {
        get;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {

        var orderService = await Locator.GetInitializedService<IOrdersService>();
        var filter = this.WhenAnyValue(x => x.IsPickup, x => x.IsDelivery, x => x.Date, x => x.SearchedText)
            .Select<(bool isPickup, bool isDelivery, DateTime date, string searchedText), Func<OrderDto, bool>>(x =>
            {
                Func<OrderDto, bool> WithSearchedText(Func<OrderDto, bool> predicate)
                {
                    return new Func<OrderDto, bool>(model => predicate(model) && IsMatch(model, x.searchedText));
                }

                if (!x.isPickup && !x.isDelivery)
                {
                    return WithSearchedText(model => model.CreatedAt.Date == x.date.Date);
                }

                if (x.isPickup && x.isDelivery)
                {
                    return WithSearchedText(model => model.CreatedAt.Date == x.date.Date);
                }

                if (x.isPickup)
                {
                    return WithSearchedText(model => model.CreatedAt.Date == x.date.Date && model.IsPickup);
                }

                if (x.isDelivery)
                {
                    return WithSearchedText(model => model.CreatedAt.Date == x.date.Date && model.IsDelivery);
                }

                return WithSearchedText(model => model.CreatedAt.Date == x.date.Date);
            });

        var streetService = await Locator.GetInitializedService<IStreetService>();
        var courierService = await Locator.GetInitializedService<ICourierService>();
        var clientService = await Locator.GetInitializedService<IClientService>();

        orderService.RuntimeOrders.BindAndFilter(filter,
            x => new OrderRowViewModel(x, streetService, courierService, clientService),
            out var collection)
            .DisposeWith(disposables);

        // Count all orders statuses

        var changeSet = collection.ToObservableChangeSet().RefCount();

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.Unconfirmed, x => x.InUncomfiredOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.New, x => x.NewOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.InCooking, x => x.InCookingOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.Ready, x => x.ReadyOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.Canceled, x => x.CanceledOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.Completed, x => x.CompletedOrders)
            .DisposeWith(disposables);

        CalculateOrderCountByStatusAndBind(changeSet, OrderStatus.OnTheWay, x => x.OnTheWayOrders)
            .DisposeWith(disposables);


        Orders = collection;
    }

    private IDisposable CalculateOrderCountByStatusAndBind(
        IObservable<IChangeSet<OrderRowViewModel>> changeSet,
        OrderStatus orderStatus,
        Expression<Func<AllDeliveriesPageVm, int>> propertyExpression)
    {
        return changeSet
                .AutoRefresh(x => x.Status)
                .Filter(x => x.Order.Status == orderStatus)
                .Count()
                .ToPropertyEx(this, propertyExpression);
    }

    private static bool IsMatch(OrderDto model, string text) => string.IsNullOrWhiteSpace(text) ||
                                                                 model.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                                                 model.LastName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                                                 model.Phone.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                                                 model.House.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                                                 model.Id.ToString("N").Contains(text, StringComparison.OrdinalIgnoreCase);
}
