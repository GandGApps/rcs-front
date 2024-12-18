﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;

public class PersonalPageVm : PageViewModel
{
    private readonly IShiftService _shiftService;
    private readonly IAuthService _authService;
    private readonly IOrdersService _ordersService;
    private readonly IMemberService _memberService;
    private readonly ObservableCollection<ShiftRowViewModel> _opennedShift = [];

    public PersonalPageVm(IShiftService shiftService, IAuthService authService, IOrdersService ordersService, IMemberService memberService)
    {
        _shiftService = shiftService;
        _authService = authService;
        _ordersService = ordersService;
        _memberService = memberService;

        TakeBreakCommand = CreatePageBusyCommand(async () =>
        {
            var pincodeDialog = new EnterPincodeDialogViewModel();

            await MainViewModel.ShowDialogAndWaitClose(pincodeDialog);

            if (string.IsNullOrWhiteSpace(pincodeDialog.Result))
            {
                return false;
            }

            var pincode = pincodeDialog.Result;

            var waitDialog = MainViewModel.ShowLoadingDialog("Проверка пинкода");

            var isCorrect = await _authService.IsManagerPincode(pincode);

            await waitDialog.CloseAsync();

            if (!isCorrect)
            {
                await MainViewModel.OkMessageAsync("Неверный пинкод");
            }
            else
            {
                await MainViewModel.OkMessageAsync("Перерыв получен");

                var shift = _shiftService.CurrentShift.Value ?? ThrowHelper.ThrowInvalidOperationException<IShift>("Shift is not started"); 
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
        }).DisposeWith(InternalDisposables);

        CloseShiftCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Закрытие смены";

            var pincodeDialog = new EnterPincodeDialogViewModel();

            await MainViewModel.ShowDialogAndWaitClose(pincodeDialog);

            if (string.IsNullOrWhiteSpace(pincodeDialog.Result))
            {
                return false;
            }

            var pincode = pincodeDialog.Result;

            var waitDialog = MainViewModel.ShowLoadingDialog("Проверка пинкода");

            var isCorrect = await _authService.IsManagerPincode(pincode);

            await waitDialog.CloseAsync();

            if (!isCorrect)
            {
                await MainViewModel.OkMessageAsync("Неверный пинкод");
            }
            else
            {
                var shift = _shiftService.CurrentShift.Value ?? ThrowHelper.ThrowInvalidOperationException<IShift>("Shift is not started");
                var orders = await _ordersService.GetOrdersOfCurrentShiftAsync();

                if (orders.Any(x => x.Status is not OrderStatus.Completed and not OrderStatus.Canceled))
                {
                    await MainViewModel.OkMessageAsync("Нельзя закрыть смену, пока есть не завершенные заказы",  "JustFailed");
                    return false;
                }

                await MainViewModel.OkMessageAsync("Смена завершена");
                await shift.End(pincode);
            }

            return isCorrect;
        }).DisposeWith(InternalDisposables);

        _shiftService.IsShiftStartedObservable()
            .Select(x => x ? CloseShiftCommand : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftCommand)
            .DisposeWith(InternalDisposables);

        _shiftService.IsShiftStartedObservable()
            .Select(x => x ? "Закрыть смену" : "Начать смену")
            .ToPropertyEx(this, x => x.ShiftButtonText)
            .DisposeWith(InternalDisposables);

        _shiftService.IsShiftStartedObservable()
            .ToPropertyEx(this, x => x.IsShiftStarted)
            .DisposeWith(InternalDisposables);
    }


    public extern bool IsShiftStarted
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public ShiftRowViewModel CurrentShift
    {
        get; set;
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
    public ReadOnlyObservableCollection<ShiftRowViewModel> ClosedShifts
    {
        get; set;
    } = null!;

    public extern IEnumerable<ShiftRowViewModel> SelectedShifts
    {
        [ObservableAsProperty]
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

    public extern string ShiftButtonText
    {
        [ObservableAsProperty]
        get;
    }

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
                var dto = shift.CreateDto();
                ShiftNumber = dto.Id.GuidToPrettyString();
                ManagerName = (await _memberService.GetMember(dto.ManagerId ?? Guid.Empty))?.Name ?? "???";
                OpennedShiftDate = dto.Start is null ? string.Empty : dto.Start.Value.ToString("dd.MM.yyyy | HH:mm");
            }

        }).DisposeWith(disposables);

        _shiftService.RuntimeShifts.BindAndFilter(x => x.End.HasValue, dto => new ShiftRowViewModel(dto, _memberService), out var cloesedShifts)
            .DisposeWith(disposables);

        ClosedShifts = cloesedShifts;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {

        if (_shiftService.CurrentShift.Value is IShift shift)
        {
            CurrentShift = new ShiftRowViewModel(shift.CreateDto(), _memberService);
        }

        IsOpennedShiftsVisible = true;

        this.WhenAnyValue(x => x.CurrentShift)
            .Subscribe(x =>
            {
                if (x is null)
                {
                    _opennedShift.Clear();
                    return;
                }

                if (_opennedShift.Count == 0)
                {
                    _opennedShift.Add(x);
                }
                else
                {
                    _opennedShift[0] = x;
                }
            })
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsOpennedShiftsVisible)
            .Select<bool, IEnumerable<ShiftRowViewModel>>(x => x ? _opennedShift : ClosedShifts)
            .ToPropertyEx(this, x => x.SelectedShifts)
            .DisposeWith(disposables);
    }
}
