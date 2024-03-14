using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
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
        });

        GoToDeliveryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var clientService = await Locator.GetInitializedService<IClientService>();

            var allClientsDialogViewModel = new AllClientsDialogViewModel(clientService)
            {
                IsDelivery = true
            };

            await MainViewModel.DialogOpenCommand.Execute(allClientsDialogViewModel).FirstAsync();
        });
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

    public ReadOnlyObservableCollection<OrderDto> Orders
    {
        get;
    }
}
