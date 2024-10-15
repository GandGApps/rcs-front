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
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class ProfileDialogViewModel : DialogViewModel
{

    public ReactiveCommand<Unit, Unit> GoToPersonalPageCommand
    {
        get;
    }

    private readonly IShiftService _shiftService;

    public ProfileDialogViewModel(IShiftService shiftService)
    {
        _shiftService = shiftService;

        GoToPersonalPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.GoToPage<PersonalPageVm>();

            await CloseAsync();
        });
    }

    [Reactive]
    public DateTime? CurrentShiftOpennedDate
    {
        get; set;
    }

    protected override void Initialize(CompositeDisposable disposables)
    {
        _shiftService.CurrentShift.Subscribe(shift =>
        {
            if (shift == null)
            {
                // TODO: add login when shift is null
            }
            else
            {
                var dto = shift.CreateDto();
                CurrentShiftOpennedDate = dto.Start;
            }

        }).DisposeWith(disposables);
    }
}
