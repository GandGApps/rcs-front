﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public class ProductViewModel(ProductDto product) : ReactiveObject
{
    public static readonly ReactiveCommand<ProductDto, Unit> AddToShoppingListCommand = ReactiveCommand.CreateFromTask<ProductDto>(async product =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var order = cashierService.CurrentOrder;

        if (order is null)
        {
            throw new InvalidOperationException("Order is not selected");
        }

        if (!(product.IsAvailable && product.IsEnoughIngredients))
        {
            return;
        }
        await order.AddProductToShoppingList(product.Id);
    });
    public Guid Id => product.Id;

    public string Name => product.Name;

    public string CurrencySymbol => product.CurrencySymbol;
    public double Price => product.Price;

    /// <summary>
    /// Need implement by IOrderEditService
    /// </summary>
    public bool IsAdded => product.IsAdded;

    public string Measure => product.Measure;

    public bool IsAvailable => product.IsAvailable && product.IsEnoughIngredients;
    public string Icon => product.Icon;
}

[EditorBrowsable(EditorBrowsableState.Never)]
public class DesignerProductViewModel : ReactiveObject
{
    public int Id
    {
        get; set;
    }
    [Reactive]
    public string Name
    {
        get; set;
    } = null!;
    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;
    [Reactive]
    public double Price
    {
        get; set;
    }
    [Reactive]
    public bool IsAdded
    {
        get; set;
    }
    [Reactive]
    public double Count
    {
        get; set;
    }
    [Reactive]
    public string Measure
    {
        get; set;
    } = null!;

    [Reactive]
    public bool IsAvailable
    {
        get; set;
    }

    [Reactive]
    public string Icon
    {
        get; set;
    } = null!;

    [Reactive]
    public bool HasIcon
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    } = null!;

    public ViewModelActivator Activator
    {
        get;
    }

    [Reactive]
    public string[] Categories
    {
        get; set;
    } = [];
}
