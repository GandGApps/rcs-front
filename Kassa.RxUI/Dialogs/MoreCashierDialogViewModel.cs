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
    public MoreCashierDialogViewModel(IOrderEditVm orderEditVm, IOrderEditService orderEditService)
    {
        AddCommentCommand = orderEditVm.CreateCommentCommand;

        StopListCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var isStopList = orderEditService.IsStopList.Value;

            orderEditService.SetIsStopList(!isStopList);

            await CloseAsync();
        });

        ShowPriceCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var isPriceShowed = orderEditService.ShowPrice.Value;

            orderEditService.SetShowPrice(!isPriceShowed);

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
