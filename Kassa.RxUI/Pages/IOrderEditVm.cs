using System.Collections.ObjectModel;
using System.Reactive;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public interface IOrderEditVm
{
    string? Category
    {
        get;
        set;
    }
    ReactiveCommand<Unit, Unit> CreateCommentCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> CreatePromocodeCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> CreateTotalCommentCommand
    {
        get;
    }
    ReadOnlyObservableCollection<ICategoryItemDto>? CurrentCategoryItems
    {
        get;
    }
    IDiscountAccesser? DiscountAccesser
    {
        get;
        set;
    }
    ReadOnlyObservableCollection<AdditiveViewModel>? FastAdditives
    {
        get;
    }
    bool IsMultiSelect
    {
        get;
        set;
    }
    ReactiveCommand<Unit, Unit> OpenDiscountsAndSurchargesDialog
    {
        get;
    }
    ReactiveCommand<Unit, Unit> OpenMoreDialogCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> OpenQuantityVolumeDialogCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> SearchAddictiveCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> SearchProductCommand
    {
        get;
    }
    ReactiveCommand<int, Unit> SelectFavouriteCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> SelectRootCategoryCommand
    {
        get;
    }
    ShoppingListViewModel? ShoppingList
    {
        get;
    }
    ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get;
    }
    string? TotalComment
    {
        get;
        set;
    }
}