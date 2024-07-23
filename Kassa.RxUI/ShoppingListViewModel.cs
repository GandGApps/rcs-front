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
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Pages;

namespace Kassa.RxUI;
public class ShoppingListViewModel : BaseViewModel
{
    private readonly ObservableCollection<ProductShoppingListItemViewModel> _productShoppingListItems = [];
    private readonly IOrderEditVm _orderEditVm;

    public ShoppingListViewModel(IOrderEditVm orderEditVm)
    {
        _orderEditVm = orderEditVm;
        ProductShoppingListItems = new(_productShoppingListItems);

        IncreaseSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {

        });

        DecreaseSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await orderEditVm.DecreaseSelectedProductShoppingListItem();
        });

        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await orderEditVm.RemoveSelectedProductShoppingListItem();
        });

        this.WhenAnyValue(x => x.Subtotal)
            .Select(x => x  -Discount)
            .Subscribe(x => Total = x);


    }

    public ReactiveCommand<Unit, Unit> IncreaseSelectedCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> DecreaseSelectedCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    public bool IsMultiSelect
    {
        get; set;
    }

    [Reactive]
    public double Discount
    {
        get; set;
    } 

    public ObservableCollection<IShoppingListItemVm> SelectedItems
    {
        get;
    } = [];


    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel> ProductShoppingListItems
    {
        get;
    }

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
