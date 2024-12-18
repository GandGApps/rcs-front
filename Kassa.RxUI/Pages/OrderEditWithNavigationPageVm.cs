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
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class OrderEditWithNavigationPageVm : PageViewModel
{
    private readonly ReadOnlyCollection<OrderEditDto> _orderEditDtos;
    private readonly Dictionary<OrderEditDto, OrderEditWithNavigationPageItemVm> _navigationItems = [];
    private readonly IIngridientsService _ingridientsService;

    public ReactiveCommand<Unit, Unit> PreviousOrderEditCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> NextOrderEditCommand
    {
        get;
    }

    [Reactive]
    public int CurrentIndex
    {
        get; private set;
    }

    public extern OrderEditWithNavigationPageItemVm Current
    {
        [ObservableAsProperty]
        get;
    }

    public OrderEditWithNavigationPageVm(ReadOnlyCollection<OrderEditDto> orderEditDtos, OrderEditDto orderEditDto, IIngridientsService ingridientsService)
    {
        _ingridientsService = ingridientsService;
        _orderEditDtos = orderEditDtos;

        var index = orderEditDtos.IndexOf(orderEditDto);
        if (index == -1)
        {
            ThrowHelper.ThrowArgumentException("Order edit not found in the list", nameof(orderEditDto));
        }

        CurrentIndex = index;

        _navigationItems[orderEditDto] = CreateOrderEditWithNavigationPageItemVm(orderEditDto);

        var previousOrderEditCommandValidation = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => index > 0);


        PreviousOrderEditCommand = CreatePageBusyCommand(async () =>
        {
            BusyText = "Загрузка...";
            if (CurrentIndex <= 0)
            {
                return;
            }
            var i = CurrentIndex - 1;
            var orderEdit = _orderEditDtos[i];

            if (!_navigationItems.TryGetValue(orderEdit, out var item))
            {
                item = CreateOrderEditWithNavigationPageItemVm(orderEdit);
                await item.InitializeAsync();
                _navigationItems[orderEdit] = item;
            }

            CurrentIndex = i;

        }, previousOrderEditCommandValidation).DisposeWith(InternalDisposables);

        var nextOrderEditCommandValidation = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => index < _orderEditDtos.Count - 1);

        NextOrderEditCommand = CreatePageBusyCommand(async() =>
        {
            if (CurrentIndex >= _orderEditDtos.Count - 1)
            {
                return;
            }
            var i = CurrentIndex + 1;
            var orderEdit = _orderEditDtos[i];

            if (!_navigationItems.TryGetValue(orderEdit, out var item))
            {
                item = CreateOrderEditWithNavigationPageItemVm(orderEdit);
                await item.InitializeAsync();
                _navigationItems[orderEdit] = item;
            }

            CurrentIndex = i;
        }, nextOrderEditCommandValidation).DisposeWith(InternalDisposables);

        InternalDisposables.Add(Disposable.Create(() =>
        {
            foreach (var navigationItem in _navigationItems.Values)
            {
                navigationItem.Dispose();
            }
        }));
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await base.InitializeAsync(disposables);

        var currentOrderEdit = _orderEditDtos[CurrentIndex];

        await _navigationItems[currentOrderEdit].InitializeAsync();

        this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => _navigationItems[_orderEditDtos[index]])
            .ToPropertyEx(this, x => x.Current)
            .DisposeWith(InternalDisposables);
    }

    private OrderEditWithNavigationPageItemVm CreateOrderEditWithNavigationPageItemVm(OrderEditDto orderEdit)
    {
        // TODO: use constructor injection
        var storageScope = _ingridientsService.CreateStorageScope();

        return RcsKassa.CreateAndInject<OrderEditWithNavigationPageItemVm>(orderEdit, storageScope);
    }

}
