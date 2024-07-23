using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class AddictiveForShoppingListItemVm : ReactiveObject, IShoppingListItemVm
{

    public AddictiveForShoppingListItemVm()
    {
        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(x =>
            {
                if (ShoppingListViewModel is null)
                {
                    return;
                }

                if (ShoppingListViewModel.IsMultiSelect)
                {
                    if (!x)
                    {
                        ShoppingListViewModel.SelectedItems.Remove(this);
                    }
                    else
                    {
                        ShoppingListViewModel.SelectedItems.Add(this);
                    }
                }
                else
                {
                    if (!x)
                    {
                        ShoppingListViewModel.SelectedItems.Remove(this);
                    }
                    else
                    {
                        ShoppingListViewModel.SelectedItems.Clear();
                        ShoppingListViewModel.SelectedItems.Add(this);
                    }
                }
            });
    }

    [Reactive]
    public string Name
    {
        get; set;
    } = null!;

    [Reactive]
    public string СurrencySymbol
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
    public string CurrencySymbol
    {
        get; set;
    } = null!;

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get; set;
    } = null!;
    [Reactive]
    public double Discount
    {
        get;
        set;
    }
    [Reactive]
    public bool HasDiscount
    {
        get;
        set;
    }
    [Reactive]
    public double SubtotalSum
    {
        get;
        set;
    }
    [Reactive]
    public double TotalSum
    {
        get;
        set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get; set;
    } = null!;

    public IShoppingListItemDto SourceDto => throw new NotImplementedException();
}
