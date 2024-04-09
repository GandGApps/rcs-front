using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class AllDeliveriesPageVm : PageViewModel
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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {

        var orderService = await Locator.GetInitializedService<IOrdersService>();
        var filter = this.WhenAnyValue(x => x.IsPickup, x => x.IsDelivery, x => x.Date)
            .Select<(bool isPickup, bool isDelivery, DateTime date), Func<OrderDto, bool>>(x =>
            {
                if (!x.isPickup && !x.isDelivery)
                {
                    return new Func<OrderDto, bool>(model => model.CreatedAt.Date == x.date.Date);
                }

                if (x.isPickup && x.isDelivery)
                {
                    return new Func<OrderDto, bool>(model => model.CreatedAt.Date == x.date.Date);
                }

                if (x.isPickup)
                {
                    return new Func<OrderDto, bool>(model => model.CreatedAt.Date == x.date.Date && model.IsPickup);
                }

                if (x.isDelivery)
                {
                    return new Func<OrderDto, bool>(model => model.CreatedAt.Date == x.date.Date && model.IsDelivery);
                }

                return new Func<OrderDto, bool>(model => model.CreatedAt.Date == x.date.Date);
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

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Неподтвержден")
            .Count()
            .ToPropertyEx(this, x => x.InUncomfiredOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Новый")
            .Count()
            .ToPropertyEx(this, x => x.NewOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Готовится")
            .Count()
            .ToPropertyEx(this, x => x.InCookingOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Готов")
            .Count()
            .ToPropertyEx(this, x => x.ReadyOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Отменен")
            .Count()
            .ToPropertyEx(this, x => x.CanceledOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "Закрытый")
            .Count()
            .ToPropertyEx(this, x => x.CompletedOrders)
            .DisposeWith(disposables);

        changeSet
            .AutoRefresh(x => x.Status)
            .Filter(x => x.Status == "В пути")
            .Count()
            .ToPropertyEx(this, x => x.OnTheWayOrders)
            .DisposeWith(disposables);

        Orders = collection;
    }
}
