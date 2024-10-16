﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using Microsoft.VisualBasic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public sealed class ServicePageVm : PageViewModel
{

    private readonly IShiftService _shiftService;
    private readonly ICashierService _cashierService;
    private readonly IOrdersService _ordersService;
    private readonly IProductService _productService;

    private readonly ObservableCollection<ServiceOrderRowViewModel> _closedOrders = [];
    private readonly ObservableCollection<ServiceOrderRowViewModel> _ordersOfClosedCashShifts = [];


    public ServicePageVm(ICashierService cashierService, IShiftService shiftService, IOrdersService ordersService, IProductService productService)
    {
        _cashierService = cashierService;
        _shiftService = shiftService;
        _ordersService = ordersService;
        _productService = productService;

        SelectedDocuments = new([]);

        ClosedOrders = new(_closedOrders);
        OrdersOfClosedCashShifts = new(_ordersOfClosedCashShifts);

        CloseShiftCommnad = CreatePageBusyCommand(async () =>
        {
            BusyText = "Закрытие смены";
            try
            {

                var orders = await _ordersService.GetOrdersOfCurrentCashierShiftAsync();

                if (orders.Any(x => x.Status is not OrderStatus.Completed and not OrderStatus.Canceled ))
                {
                    this.Log().Error("There are not completed orders in the current cashier shift");

                    await MainViewModel.OkMessageAsync("Не все заказы закрыты или завершены!", "JustFailed");

                    return false;
                }

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

        OpenShiftCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Открытие смены";
            try
            {
                if (_shiftService.CurrentCashierShift.Value is null)
                {
                    this.Log().Error("Shift is null");
                    return false;
                }

                if (!_shiftService.IsCashierShiftStarted())
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

        WithdrawMoneyCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = RcsKassa.CreateAndInject<SeizureReasonDialogViewModel>();

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        DepositMoneyCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = RcsKassa.CreateAndInject<ContributionReasonDialogViewModel>();

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        OpenClosedOrderCommand = CreatePageBusyCommand<ServiceOrderRowViewModel, Unit>(async order =>
        {
            BusyText = "Открытие заказа";

            var closedOrders = new ServiceOrderRowViewModel[ClosedOrders!.Count];
            ClosedOrders!.CopyTo(closedOrders, 0);

            var orderEdits = new OrderEditDto[closedOrders.Length];

            for(var i = 0; i < closedOrders.Length; i++)
            {
                orderEdits[i] = await _cashierService.CreateOrder(closedOrders[i].Order!);
            }

            var closedOrder = orderEdits.First(x => order.Id == x.Id);
            var orderEditWithNavigationPageVm = RcsKassa.CreateAndInject<OrderEditWithNavigationPageVm>(new ReadOnlyCollection<OrderEditDto>(orderEdits), closedOrder);

            await MainViewModel.GoToPage(orderEditWithNavigationPageVm);

            return Unit.Default;
        });

        OpenOfClosedShiftOrderCommand = CreatePageBusyCommand<ServiceOrderRowViewModel, Unit>(async order =>
        {
            BusyText = "Открытие заказа";

            var closedOrders = new ServiceOrderRowViewModel[OrdersOfClosedCashShifts!.Count];
            OrdersOfClosedCashShifts!.CopyTo(closedOrders, 0);

            var orderEdits = new OrderEditDto[closedOrders.Length];

            for (var i = 0; i < closedOrders.Length; i++)
            {
                orderEdits[i] = await _cashierService.CreateOrder(closedOrders[i].Order!);
            }

            var closedOrder = orderEdits.First(x => order.Id == x.Id);
            var orderEditWithNavigationPageVm = RcsKassa.CreateAndInject<OrderEditWithNavigationPageVm>(new ReadOnlyCollection<OrderEditDto>(orderEdits), closedOrder);

            await MainViewModel.GoToPage(orderEditWithNavigationPageVm);

            return Unit.Default;
        });

        OpenOpenOrderCommand = CreatePageBusyCommand<ServiceOrderRowViewModel, Unit>(async order =>
        {
            BusyText = "Открытие заказа";

            var openOrders = new OrderEditDto[_cashierService.Orders.Count];
            _cashierService.Orders.CopyTo(openOrders, 0);

            var orderEdit = openOrders.First(x => order.Id == x.Id);
            var orderEditWithNavigationPageVm = RcsKassa.CreateAndInject<OrderEditWithNavigationPageVm>(new ReadOnlyCollection<OrderEditDto>(openOrders), orderEdit);

            await MainViewModel.GoToPage(orderEditWithNavigationPageVm);

            return Unit.Default;
        });

        _shiftService.IsCashierShiftStartedObservable()
            .ToPropertyEx(this, x => x.IsShiftOpenned)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsShiftOpenned)
            .Select(x => !x ? "Открыть кассовую смену" : "Закрыть кассовую смену")
            .ToPropertyEx(this, x => x.CashierShiftButtonText)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsShiftOpenned)
            .Select(x => x ? CloseShiftCommnad : OpenShiftCommand)
            .ToPropertyEx(this, x => x.ShiftButtonCommand)
            .DisposeWith(InternalDisposables);

        var disposables = new CompositeDisposable();

        _shiftService.IsCashierShiftStartedObservable()
            .Subscribe(x =>
            {

                if (disposables.Count > 0)
                {
                    disposables.Dispose();
                    disposables = [];
                }

                if (x)
                {
                    _closedOrders.Clear();
                    _ordersOfClosedCashShifts.Clear();

                    // Closed orders in the current cashier shift
                    InitAndFilterAndSelectThenSubscribeToRuntimes(x =>
                    {
                        Debug.Assert(_shiftService.CurrentCashierShift.Value != null);

                        var cashierShiftId = _shiftService.CurrentCashierShift.Value.CreateDto().Id;

                        if (x.CashierShiftId == cashierShiftId && x.PaymentInfo != null)
                        {
                            return true;
                        }

                        return false;
                    },
                        x => new ServiceOrderRowViewModel(x, shiftService, _productService, _cashierService),
                        ordersService.RuntimeOrders,
                        _closedOrders)
                        .DisposeWith(disposables);

                    // Closed orders in the closed cashier shifts
                    InitAndFilterAndSelectThenSubscribeToRuntimes(x =>
                    {
                        Debug.Assert(_shiftService.CurrentCashierShift.Value != null);

                        var cashierShiftId = _shiftService.CurrentCashierShift.Value.CreateDto().Id;

                        if (x.CashierShiftId != cashierShiftId && x.PaymentInfo != null)
                        {
                            return true;
                        }

                        return false;
                    },
                        x => new ServiceOrderRowViewModel(x, shiftService, _productService, _cashierService),
                        ordersService.RuntimeOrders,
                        _ordersOfClosedCashShifts)
                        .DisposeWith(disposables);
                }

            }).DisposeWith(InternalDisposables);

        _cashierService.Orders.ToObservableChangeSet()
            .Transform(x => new ServiceOrderRowViewModel(x, shiftService, _productService, _cashierService))
            .Bind(out var openOrders)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(InternalDisposables);

        OpenOrders = openOrders;

        
    }


    public ReadOnlyObservableCollection<ServiceOrderRowViewModel> OpenOrders
    {
        get;
    }

    public ReadOnlyObservableCollection<ServiceOrderRowViewModel> ClosedOrders
    {
        get;
    }

    public ReadOnlyObservableCollection<ServiceOrderRowViewModel> OrdersOfClosedCashShifts
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

    public ReactiveCommand<Unit, Unit> WithdrawMoneyCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> DepositMoneyCommand
    {
        get;
    }

    public ReactiveCommand<ServiceOrderRowViewModel, Unit> OpenOpenOrderCommand
    {
        get;
    }

    public ReactiveCommand<ServiceOrderRowViewModel, Unit> OpenClosedOrderCommand
    {
        get;
    }

    public ReactiveCommand<ServiceOrderRowViewModel, Unit> OpenOfClosedShiftOrderCommand
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


    private static IDisposable InitAndFilterAndSelectThenSubscribeToRuntimes<T1, T2>(Func<T1, bool> filter, Func<T1, T2> selector, IApplicationModelManager<T1> manager, ObservableCollection<T2> target) 
        where T1 : class, IGuidId
        where T2: class, IGuidId
    {
        var first = manager.Values.Where(filter).Select(selector);

        target.AddRange(first);

        return manager.Subscribe(changeSet =>
        {
            foreach (var change in changeSet.Changes)
            {
                var model = change.Current;

                if (!filter(model))
                {
                    continue;
                }

                switch (change.Reason)
                {

                    case ModelChangeReason.Add:
                        var added = selector(model);
                        target.Add(added);
                        break;

                    case ModelChangeReason.Replace:
                        var replaced = target.FirstOrDefault(x => x.Id == model.Id);
                        if (replaced != null)
                        {
                            target.Remove(replaced);
                        }

                        var newModel = selector(model);
                        target.Add(newModel);
                        break;


                    case ModelChangeReason.Remove:
                        var removed = target.FirstOrDefault(x => x.Id == model.Id);
                        if (removed != null)
                        {
                            target.Remove(removed);
                        }
                        break;

                }
            }

        });
    }

}
