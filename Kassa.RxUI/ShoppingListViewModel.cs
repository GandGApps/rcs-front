using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Aggregation;
using Kassa.BuisnessLogic;
using Splat;

namespace Kassa.RxUI;
public class ShoppingListViewModel : ReactiveObject
{
    private readonly ICashierService _cashierService = Locator.Current.GetRequiredService<ICashierService>();

    public ShoppingListViewModel()
    {


        IncreaseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            foreach (var item in CurrentItems)
            {
                if (item is ShoppingListItemViewModel itemViewModel)
                {
                    var product = await _cashierService.GetProductById(itemViewModel.Id);

                    // Check if product is available
                    if (product is null || product.Count == 0)
                    {
                        continue;
                    }

                    // Take product from storage
                    await _cashierService.DecreaseProductCount(itemViewModel.Id);
                }
                item.Count++;
            }
        });

        DecreaseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            foreach (var item in CurrentItems)
            {
                if (item is ShoppingListItemViewModel itemViewModel)
                {
                    if (item.Count == 1)
                    {
                        await item.RemoveCommand.Execute();
                    }

                    // Return product to storage
                    await _cashierService.IncreaseProductCount(itemViewModel.Id);
                }
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

        AddictiveViewModels
            .ToObservableChangeSet()
            .AutoRefresh(x => x.SubtotalSum)
            .ToCollection()
            .Select(list => list.Sum(item => item.SubtotalSum * (item.HasDiscount ? item.Discount : 1)))
            .Subscribe(x => Total = x);
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
