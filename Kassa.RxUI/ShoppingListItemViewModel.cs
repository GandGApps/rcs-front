using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class ShoppingListItemViewModel : ReactiveObject, IShoppingListItem
{
    /// <summary>
    /// Need's for design time
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShoppingListItemViewModel() : this(new ShoppingListViewModel())
    {

    }

    public ShoppingListItemViewModel(ShoppingListViewModel shoppingListViewModel)
    {
        ShoppingListViewModel = shoppingListViewModel;


        this.WhenAnyValue(x => x.IsChecked)
            .Subscribe(x =>
            {
                if (ShoppingListViewModel.IsMultiSelect)
                {
                    if (!x)
                    {
                        ShoppingListViewModel.CurrentItems.Remove(this);
                    }
                    else
                    {
                        ShoppingListViewModel.CurrentItems.Add(this);
                    }
                }
                else
                {
                    if (!x)
                    {
                        ShoppingListViewModel.CurrentItems.Remove(this);
                    }
                    else
                    {
                        ShoppingListViewModel.CurrentItems.Clear();
                        ShoppingListViewModel.CurrentItems.Add(this);
                    }
                }
            });

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveSubtotalSum)
            .Select(x => (x.Item1 * x.Item2) + x.Item3)
            .Subscribe(x => SubtotalSum = x);

        Addictives
            .ToObservableChangeSet()
            .AutoRefresh(x => x.Price)
            .ToCollection()
            .Select(list => list.Sum(item => item.Price))
            .Subscribe(x => AddictiveSubtotalSum = x);

        RemoveCommand = ReactiveCommand.Create(() =>
        {
            ShoppingListViewModel.AddictiveViewModels.Remove(this);
        });
    }

    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get; set;
    }

    [Reactive]
    public bool HasDiscount
    {
        get; set;
    }

    [Reactive]
    public double Discount
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
    public string Name
    {
        get; set;
    } = null!;

    [Reactive]
    public double Price
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;

    [Reactive]
    public string AddictiveInfo
    {
        get; set;
    } = null!;

    public ObservableCollection<AddictiveForShoppingListItem> Addictives
    {
        get;
    } = [];

    [Reactive]
    public bool HasAddictiveInfo
    {
        get; set;
    }
    [Reactive]
    public bool IsChecked
    {
        get; set;
    }

    [Reactive]
    public double AddictiveSubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double SubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double TotalSum
    {
        get; set;
    }
    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }
}
