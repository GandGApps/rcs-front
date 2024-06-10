﻿using System.Collections.ObjectModel;
using System.Reactive;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public interface IOrderEditVm: IReactiveObject
{
    public string? Category
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> CreateCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreatePromocodeCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreateTotalCommentCommand
    {
        get;
    }

    public ReadOnlyObservableCollection<ICategoryItemDto>? CurrentCategoryItems
    {
        get;
    }

    public IDiscountAccesser? DiscountAccesser
    {
        get; set;
    }
    
    public ReadOnlyObservableCollection<AdditiveViewModel>? FastAdditives
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDiscountsAndSurchargesDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenMoreDialogCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenQuantityVolumeDialogCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SearchAddictiveCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SearchProductCommand
    {
        get;
    }

    public ReactiveCommand<int, Unit> SelectFavouriteCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SelectRootCategoryCommand
    {
        get;
    }

    public ShoppingListViewModel? ShoppingList
    {
        get;
    }

    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get;
    }

    public string? TotalComment
    {
        get;
    }
}