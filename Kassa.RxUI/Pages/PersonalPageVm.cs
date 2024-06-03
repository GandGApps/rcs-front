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
using Kassa.BuisnessLogic.ApplicationModelManagers;
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
    
    private readonly ObservableCollection<ShiftRowViewModel> _selectedShifts = [];

    public PersonalPageVm(IShiftService shiftService)
    {
        _shiftService = shiftService;

        TakeBreakCommand = CreatePageBusyCommand(async () =>
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

        OpenShiftCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Открытие смены";

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

        CloseShiftCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Закрытие смены";

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

                await shift.End(pincode);
            }

            return isCorrect;
        }).DisposeWith(InternalDisposables);

        SelectedShifts = new(_selectedShifts);

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

        _shiftService.IsShiftStartedObservable()
            .Select(x => x ? CloseShiftCommand : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftCommand);

    }

    [Reactive]
    public string ManagerName
    {
        get; set;
    }

    [Reactive]
    public string CashierName
    {
        get; set;
    }

    [Reactive]
    public string ShiftNumber
    {
        get; set;
    }

    [Reactive]
    public string OpennedShiftDate
    {
        get; set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ShiftRowViewModel> SelectedShifts
    {
        get; set;
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

    [Reactive]
    public bool IsOpennedShifts
    {
        get; set;
    } = true;

    protected override void Initialize(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.IsOpennedShiftsVisible, x => !x)
            .ToPropertyEx(this, x => x.IsClosedShiftsVisible)
            .DisposeWith(disposables);

        _shiftService.CurrentShift.Subscribe(async shift =>
        {

            if (shift is null)
            {
                ManagerName = "???";
                CashierName = "???";
                ShiftNumber = "???";
            }
            else
            {
                CashierName = shift.Member.Name;
                var dto = await shift.CreateDto();
                ShiftNumber = dto.Id.GuidToPrettyString();
                ManagerName = dto.ManagerId.GuidToPrettyString();
                OpennedShiftDate = dto.Start is null ? "???" : dto.Start.Value.ToString("dd.MM.yyyy | HH:mm");
            }

        }).DisposeWith(disposables);


        var isOpennedShifts = this.WhenAnyValue(x => x.IsOpennedShifts)
                                  .Select(x =>
                                  {
                                  if (x)
                                  {
                                      return new Func<ShiftDto, bool>(x => x.End is null || x.End.Value.Date != DateTime.Today);
                                  }
                                  else
                                  {
                                      return new Func<ShiftDto, bool>(x => x.Start is null || x.Start.Value.Date == DateTime.Today || x.IsStarted);
                                      }
                                  });

        _shiftService.RuntimeShifts.BindAndFilter(isOpennedShifts, dto => new ShiftRowViewModel(dto), out var collection)
            .DisposeWith(disposables);

        SelectedShifts = collection;
    }
}
