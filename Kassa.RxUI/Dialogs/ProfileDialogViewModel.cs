using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class ProfileDialogViewModel : DialogViewModel
{
    public ReactiveCommand<Unit, Unit> GoToPersonalPageCommand
    {
        get;
    }

    public ProfileDialogViewModel()
    {
        GoToPersonalPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftSerivce = await Locator.GetInitializedService<IShiftService>();

            var personnelPage = new PersonalPageVm(shiftSerivce);

            await MainViewModel.GoToPage(personnelPage);

            await CloseAsync();
        });
    }
}
