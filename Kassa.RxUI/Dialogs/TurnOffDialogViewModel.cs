using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class TurnOffDialogViewModel : DialogViewModel
{
    public TurnOffDialogViewModel()
    {
        LogoutCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = await Locator.GetInitializedService<IShiftService>();

            if (shiftService.CurrentShift.Value == null)
            {
                throw new InvalidOperationException("Shift is not started");
            }

            await shiftService.CurrentShift.Value!.Exit();

            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand
    {
        get;
    }
}
