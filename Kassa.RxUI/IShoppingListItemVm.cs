using System.Reactive;
using System.Windows.Input;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;

namespace Kassa.RxUI;

public interface IShoppingListItemVm: IReactiveObject
{
    double Count
    {
        get;
        set;
    }
    string CurrencySymbol
    {
        get;
        set;
    }
    double Discount
    {
        get;
        set;
    }
    bool HasDiscount
    {
        get;
        set;
    }
    bool IsSelected
    {
        get;
        set;
    }
    string Measure
    {
        get;
        set;
    }
    string Name
    {
        get;
        set;
    }
    double Price
    {
        get;
        set;
    }
    ShoppingListViewModel ShoppingListViewModel
    {
        get;
        set;
    }
    double SubtotalSum
    {
        get;
        set;
    }
    double TotalSum
    {
        get;
        set;
    }

    ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get; 
    }

    /// <summary>
    /// If the item is ordered, this property contains the id of the ordered item. See <see cref="OrderedProductDto"/> or <see cref="OrderedAdditiveDto"/>
    /// </summary>
    Guid OrderedId
    {
        get;
    }
}