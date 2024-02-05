﻿using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Aggregation;
using Kassa.BuisnessLogic;
using Splat;
using Kassa.BuisnessLogic.Services;

namespace Kassa.RxUI;
public class ShoppingListViewModel : ReactiveObject
{

    public ShoppingListViewModel(ICashierService cashierService)
    {
        IncreaseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await cashierService.IncreaseSelectedProductShoppingListItem();
        });

        DecreaseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await cashierService.DecreaseSelectedProductShoppingListItem();
        });

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await cashierService.RemoveSelectedProductShoppingListItem();
        });
    }

    public ObservableCollection<ProductShoppingListItemViewModel> AddictiveViewModels
    {
        get;
    } = [];

    public ReactiveCommand<Unit, Unit> IncreaseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> DecreaseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    [Reactive]
    public bool IsMultiSelect
    {
        get; set;
    }

    [Reactive]
    public double Discount
    {
        get; set;
    }

    public ObservableCollection<IShoppingListItem> CurrentItems
    {
        get;
    } = [];

    [Reactive]
    public double Subtotal
    {
        get; set;
    }

    [Reactive]
    public double Total
    {
        get; set;
    }
}
