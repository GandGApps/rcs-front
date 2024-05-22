using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public class ServicePageVm : PageViewModel
{
    private readonly IShiftService _shiftService;
    private readonly ICashierService _cashierService;

    public ServicePageVm(ICashierService cashierService, IShiftService shiftService)
    {
        _cashierService = cashierService;
        _shiftService = shiftService;

        SelectedOrders = new([]);
        SelectedDocuments = new([]);

        CloseShiftCommnad = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                if (_shiftService.IsCashierShiftStarted())
                {
                    // if shift started, then it's not null
                    await _shiftService.CurrentCashierShift.Value!.End();

                    return true;
                }

                this.Log().Error("Shift is not started");

                return false;
            }
            catch (Exception e)
            {
                this.Log().Error(e, "Error on close shift");
                return false;
            }
        });

        OpenShiftCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                if (_shiftService.CurrentCashierShift.Value is null)
                {
                    this.Log().Error("Shift is null");
                    return false;
                }

                if (!_shiftService.IsShiftStarted())
                {
                    await _shiftService.CurrentCashierShift.Value.Start();
                    return true;
                }

                this.Log().Error("Shift is already started");

                return false;
            }
            catch (Exception e)
            {

                this.Log().Error(e, "Error on open shift");
                return false;
            }
        });

        this.WhenAnyValue(x => x.IsShiftOpenned)
            .Select(x => x ? CloseShiftCommnad : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftButtonCommand)
            .DisposeWith(InternalDisposables);

        _shiftService.IsCashierShiftStartedObservable()
            .ToPropertyEx(this, x => x.IsShiftOpenned)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsShiftOpenned)
            .Select(x => !x ? "Открыть кассовую смену" : "Закрыть кассовую смену")
            .ToPropertyEx(this, x => x.CashierShiftButtonText)
            .DisposeWith(InternalDisposables);
    }

    public ReadOnlyObservableCollection<ServiceOrderRowViewModel> SelectedOrders
    {
        get;
    }

    public ReadOnlyObservableCollection<DocumentRowViewModel> SelectedDocuments
    {
        get;
    }

    [Reactive]
    public bool IsRemoveOrderChecked
    {
        get; set;
    }

    public extern ReactiveCommand<Unit, bool> ShiftButtonCommand
    {
        [ObservableAsProperty]
        get;
    }

    public ReactiveCommand<Unit, bool> CloseShiftCommnad
    {
        get;
    }

    public ReactiveCommand<Unit, bool> OpenShiftCommand
    {
        get;
    }

    public extern string CashierShiftButtonText
    {
        [ObservableAsProperty]
        get;
    }

    public extern bool IsShiftOpenned
    {
        [ObservableAsProperty]
        get; 
    } 

}
