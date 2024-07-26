using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class MoreCashierDialogViewModel : DialogViewModel
{
    public MoreCashierDialogViewModel(IOrderEditVm orderEditVm)
    {
        AddCommentCommand = orderEditVm.CreateCommentCommand;

        StopListCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            orderEditVm.IsStopList = !orderEditVm.IsStopList;

            await CloseAsync();
        });

        ShowPriceCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            orderEditVm.IsShowPrice = !orderEditVm.IsShowPrice;

            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> AddCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> StopListCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> ShowPriceCommand
    {
        get;
    }
}
