using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

    [Reactive]
    public DateTime? CurrentShiftOpennedDate
    {
        get; set;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var shiftService = await Locator.GetInitializedService<IShiftService>();

        shiftService.CurrentShift.Subscribe(async shift =>
        {
            if (shift == null)
            {
                // TODO: add login when shift is null
            }
            else
            {
                var dto = await shift.CreateDto();
                CurrentShiftOpennedDate = dto.Start;
            }
        }).DisposeWith(disposables);
    }
}
