using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;

public class PersonalPageVm : PageViewModel
{
    private readonly IShiftService _shiftService;

    public PersonalPageVm(IShiftService shiftService)
    {
        _shiftService = shiftService;

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

                var shift = _shiftService.CurrentShift.Value;

                if (shift is null)
                {
                    throw new InvalidOperationException("Shift is not started");
                }

                await shift.TakeBreak(pincode);
            }

            return isCorrect;
        }).DisposeWith(InternalDisposables);

        OpenShiftCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var currentShift = _shiftService.CurrentShift.Value;

            if (currentShift is null)
            {
                return false;
            }

            try
            {
                await currentShift.Start();
            }
            catch
            {
                return false;
            }

            return true;
        });

        CloseShiftCommand = ReactiveCommand.CreateFromTask(async () =>
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

                var shift = _shiftService.CurrentShift.Value;

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
                Begin = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                End = DateTime.Now.AddHours(8).ToString("dd/MM/yyyy HH:mm"),
                Break = "",
                Earned = 800,
                Fine = 0,
                Comment = "asdasd",
                Manager = "Петров Петр Петрович"
            }
        ]
        );

        ShiftButtonText = "Начать смену";

        _shiftService.CurrentShift
            .Subscribe(shift =>
            {
                if (shift == null)
                {
                    ShiftButtonText = "Начать смену";
                    return;
                }
                shift.IsStarted.Subscribe(isStarted =>
                {
                    ShiftButtonText = isStarted ? "Завершить смену" : "Начать смену";
                }).DisposeWith(InternalDisposables);
            })
            .DisposeWith(InternalDisposables);

        _shiftService.CurrentShift
            .SelectMany(x => x is null ? Observable.Return<bool?>(null) : x.IsStarted.Select(x => (bool?)x))
            .Select(x => x.HasValue && x.Value)
            .Select(x => x ? CloseShiftCommand : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftCommand);

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

    public ReactiveCommand<Unit, bool> CloseShiftCommand
    {
        get;
    }

    public ReactiveCommand<Unit, bool> OpenShiftCommand
    {
        get;
    }

    public extern ReactiveCommand<Unit, bool> ShiftCommand
    {
        [ObservableAsProperty]
        get; 
    }

    [Reactive]
    public string ShiftButtonText
    {
        get; set;
    }

    protected override void Initialize(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.IsOpennedShiftsVisible, x => !x)
            .ToPropertyEx(this, x => x.IsClosedShiftsVisible)
            .DisposeWith(disposables);

    }

}
