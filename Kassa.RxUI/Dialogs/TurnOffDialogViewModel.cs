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
    public TurnOffDialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        LogoutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.GoToPageAndResetCommand.Execute(new PincodePageVm(mainViewModel)).FirstAsync();

            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand
    {
        get;
    }
}
