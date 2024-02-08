using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class AreYouSureToTurnOffDialogViewModel : DialogViewModel
{

    public AreYouSureToTurnOffDialogViewModel(MainViewModel mainViewModel)
    {
        TurnOffCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.CloseCommand.Execute().FirstAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> TurnOffCommand
    {
        get;
    }
}
