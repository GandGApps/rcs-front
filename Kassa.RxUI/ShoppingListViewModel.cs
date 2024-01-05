using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Aggregation;

namespace Kassa.RxUI;
public class ShoppingListViewModel : ReactiveObject
{
    public ShoppingListViewModel()
    {
        IncreaseCommand = ReactiveCommand.Create(() =>
        {
            foreach (var item in CurrentItems)
            {
                item.Count++;
            }
        });

        DecreaseCommand = ReactiveCommand.Create(() =>
        {
            foreach (var item in CurrentItems)
            {
                item.Count--;
            }
        });

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var items = CurrentItems.ToArray();

            foreach (var item in items)
            {
                await item.RemoveCommand.Execute();
            }
        });

        AddictiveViewModels
            .ToObservableChangeSet()
            .AutoRefresh(x => x.SubtotalSum)
            .ToCollection()
            .Select(list => list.Sum(item => item.SubtotalSum))
            .Subscribe(x => Subtotal = x);
    }

    public ObservableCollection<ShoppingListItemViewModel> AddictiveViewModels
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
