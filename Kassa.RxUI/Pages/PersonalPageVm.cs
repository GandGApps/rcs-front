using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class PersonalPageVm : PageViewModel
{

    public PersonalPageVm()
    {
        TakeBreakCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var pincodeDialog = new EnterPincodeDialogViewModel();
            var authentificationService = Locator.GetRequiredService<IAuthService>();

            await MainViewModel.ShowDialogAndWaitClose(pincodeDialog);

            if (string.IsNullOrWhiteSpace(pincodeDialog.Result))
            {
                return false;
            }

            var pincode = pincodeDialog.Result;

            var waitDialog = MainViewModel.ShowLoadingDialog("Проверка пинкода");

            var isCorrect = await authentificationService.IsManagerPincode(pincode);

            await waitDialog.CloseAsync();

            if (!isCorrect)
            {
                await MainViewModel.OkMessage("Неверный пинкод");
            }
            else
            {
                await MainViewModel.OkMessage("Перерыв получен");

                var shiftService = await Locator.GetInitializedService<IShiftService>();

                var shift = shiftService.CurrentShift.Value;

                if (shift is null)
                {
                    throw new InvalidOperationException("Shift is not started");
                }

                await shift.TakeBreak();
            }

            return isCorrect;
        }).DisposeWith(InternalDisposables);

        EndShiftCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var pincodeDialog = new EnterPincodeDialogViewModel();
            var authentificationService = Locator.GetRequiredService<IAuthService>();

            await MainViewModel.ShowDialogAndWaitClose(pincodeDialog);

            if (string.IsNullOrWhiteSpace(pincodeDialog.Result))
            {
                return false;
            }

            var pincode = pincodeDialog.Result;

            var waitDialog = MainViewModel.ShowLoadingDialog("Проверка пинкода");

            var isCorrect = await authentificationService.IsManagerPincode(pincode);

            await waitDialog.CloseAsync();

            if (!isCorrect)
            {
                await MainViewModel.OkMessage("Неверный пинкод");
            }
            else
            {
                await MainViewModel.OkMessage("Смена завершена");

                var shiftService = await Locator.GetInitializedService<IShiftService>();

                var shift = shiftService.CurrentShift.Value;

                if (shift is null)
                {
                    throw new InvalidOperationException("Shift is not started");
                }

                await shift.Exit();
            }

            return isCorrect;
        }).DisposeWith(InternalDisposables);

        SelectedShifts = new(
        [
            new() {
                HourlyRate = 100,
                Name = "Иванов Иван Иванович",
                Begin = DateTime.Now.ToString(),
                End = DateTime.Now.AddHours(8).ToString(),
                Break = "",
                Earned = 800,
                Fine = 0,
                Comment = "asdasd",
                Manager = "Петров Петр Петрович"
            }
        ]
        );
    }

    public ReadOnlyObservableCollection<ShiftRowViewModel> SelectedShifts
    {
        get;
    }

    [Reactive]
    public bool IsOpennedShiftsVisible
    {
        get; set;
    }

    public extern bool IsClosedShiftsVisible
    {
        [ObservableAsProperty]
        get;
    }

    public ReactiveCommand<Unit, bool> TakeBreakCommand
    {
        get;
    }

    public ReactiveCommand<Unit, bool> EndShiftCommand
    {
        get;
    }

    protected override void Initialize(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.IsOpennedShiftsVisible, x => !x)
            .ToPropertyEx(this, x => x.IsClosedShiftsVisible)
            .DisposeWith(disposables);
    }
}
