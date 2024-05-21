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
    private readonly ICashierService _cashierService;

    public ServicePageVm(ICashierService cashierService)
    {
        _cashierService = cashierService;

        SelectedOrders = new([]);
        SelectedDocuments = new([]);

        CloseShiftCommnad = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                if (_cashierService.IsShiftStarted())
                {
                    // if shift started, then it's not null
                    await _cashierService.CurrentShift.Value!.End();

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
                if (_cashierService.CurrentShift.Value is null)
                {
                    this.Log().Error("Shift is null");
                    return false;
                }

                if (!_cashierService.IsShiftStarted())
                {
                    await _cashierService.CurrentShift.Value.Start();
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

        this.WhenAnyValue(x => x.IsOpennedShifts)
            .Select(x => x ? CloseShiftCommnad : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftButtonCommand)
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

    [Reactive]
    public bool IsOpennedShifts
    {
        get; set;
    } = true;

    protected override void Initialize(CompositeDisposable disposables)
    {
        _cashierService.IsShiftStartedObservable()
            .ToPropertyEx(this, x => x.IsOpennedShifts)
            .DisposeWith(disposables);


    }
}
