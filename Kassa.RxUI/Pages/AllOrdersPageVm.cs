using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class AllOrdersPageVm : PageViewModel
{
    public AllOrdersPageVm()
    {
        MoveDateCommand = ReactiveCommand.Create<int>(x =>
        {
            Date = Date.AddDays(x);
        });
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
}
