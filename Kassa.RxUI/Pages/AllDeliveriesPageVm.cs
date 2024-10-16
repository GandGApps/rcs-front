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
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class AllDeliveriesPageVm : PageViewModel
{
    private readonly IClientService _clientService;
    private readonly ICourierService _courierService;
    private readonly IDistrictService _districtService;
    private readonly IStreetService _streetService;
    private readonly IOrdersService _ordersService;

    public AllDeliveriesPageVm(IClientService clientService, ICourierService courierService, IDistrictService districtService, IStreetService streetService, IOrdersService ordersService)
    {
        _clientService = clientService;
        _courierService = courierService;
        _districtService = districtService;
        _streetService = streetService;
        _ordersService = ordersService;

        Date = DateTime.UtcNow;

        MoveDateCommand = ReactiveCommand.Create<int>(x =>
        {
            Date = Date.AddDays(x);
        });

        GoToPickUpCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allClientsDialogViewModel = RcsKassa.CreateAndInject<AllClientsDialogViewModel>();
            allClientsDialogViewModel.IsPickup = true;

            await MainViewModel.ShowDialogAsync(allClientsDialogViewModel);
        }).DisposeWith(InternalDisposables);

        GoToDeliveryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allClientsDialogViewModel = RcsKassa.CreateAndInject<AllClientsDialogViewModel>();
            allClientsDialogViewModel.IsDelivery = true;

            await MainViewModel.ShowDialogAsync(allClientsDialogViewModel);
        }).DisposeWith(InternalDisposables);

        EditOrderCommand = ReactiveCommand.CreateFromTask<OrderDto>(async order =>
        {
            var client = order.ClientId.HasValue ? await _clientService.GetClientById(order.ClientId.Value) : null;
            var courier = order.CourierId.HasValue ? await _courierService.GetCourierById(order.CourierId.Value) : null;
            var district = order.DistrictId.HasValue ? await _districtService.GetDistrictById(order.DistrictId.Value) : null;
            var street = order.StreetId.HasValue ? await _streetService.GetStreetById(order.StreetId.Value) : null;

            var editPage = EditDeliveryPageVm.CreatePage(client, courier, order, district, street);

            await MainViewModel.GoToPage(editPage);

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

    protected override void Initialize(CompositeDisposable disposables)
    {
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

        _ordersService.RuntimeOrders.BindAndFilter(filter,
            x => new OrderRowViewModel(x, _streetService, _courierService, _clientService),
            out var collection)
            .DisposeWith(disposables);

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

    private ObservableAsPropertyHelper<int> CalculateOrderCountByStatusAndBind(
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
