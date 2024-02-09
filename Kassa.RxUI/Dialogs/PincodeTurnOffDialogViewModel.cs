using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class PincodeTurnOffDialogViewModel : DialogViewModel
{
    public PincodeTurnOffDialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        TurnOffCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.DialogOpenCommand.Execute(new AreYouSureToTurnOffDialogViewModel(mainViewModel)).FirstAsync();
        });

        BackToLoginPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.GoBackCommand.Execute().FirstAsync();
            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> TurnOffCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> BackToLoginPageCommand
    {
        get;
    }
}
