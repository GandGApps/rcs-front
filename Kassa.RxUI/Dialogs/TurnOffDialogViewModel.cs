using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class TurnOffDialogViewModel : DialogViewModel
{
    public TurnOffDialogViewModel()
    {
        LogoutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.GoToPageAndResetCommand.Execute(new PincodePageVm(MainViewModel)).FirstAsync();

            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand
    {
        get;
    }
}
