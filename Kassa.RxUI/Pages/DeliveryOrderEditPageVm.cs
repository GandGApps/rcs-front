﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using DynamicData.Binding;
using Kassa.RxUI.Dialogs;
using System.Reactive.Linq;
using DynamicData;

namespace Kassa.RxUI.Pages;
public sealed class DeliveryOrderEditPageVm : PageViewModel, IOrderEditVm
{
    private readonly IOrderEditService _orderEditService;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;
    private readonly IProductService _productService;

    public DeliveryOrderEditPageVm(IOrderEditService orderEditService, ICashierService cashierService, IAdditiveService additiveService, IProductService productService)
    {
        _orderEditService = orderEditService;
        _cashierService = cashierService;
        _additiveService = additiveService;

        CreateTotalCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this)
            {
                Comment = TotalComment
            };

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished)
            {
                await _orderEditService!.WriteTotalComment(dialog.Comment);
                TotalComment = dialog.Comment;
            }
        });

        CreatePromocodeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var promo = new PromocodeDialogViewModel(this);
            promo.ApplyCommand.Subscribe(x =>
            {
                DiscountAccesser = x;
            });
            var dialog = await MainViewModel.DialogOpenCommand.Execute(promo).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SearchAddictiveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var additiveService = await Locator.GetInitializedService<IAdditiveService>();
            var addictiveDialog = new SearchAddictiveDialogViewModel(additiveService, orderEditService);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SelectFavouriteCommand = ReactiveCommand.CreateFromTask(async (int x) =>
        {
            if (_orderEditService == null)
            {
                return;
            }
            await _orderEditService.SelectFavourite(x);
        });

        SelectRootCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (_orderEditService == null)
            {

                return;
            }
            await _orderEditService.SelectRootCategory();
        });

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var searchProductDialog = new SearchProductDialogViewModel(orderEditService, productService);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished && _orderEditService is not null)
            {
                await _orderEditService.WriteCommentToSelectedItems(dialog.Comment);
            }
        });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var dialog = new MoreCashierDialogViewModel(this, _orderEditService);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        OpenDiscountsAndSurchargesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new DiscountsAndSurchargesDialogViewModel();

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        OpenQuantityVolumeDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (ShoppingListItems == null)
            {
                return;
            }
            var shoppingListItem = ShoppingListItems.FirstOrDefault(x => x.IsSelected);

            if (shoppingListItem == null)
            {
                return;
            }

            var dialog = new QuantityVolumeDialogVewModel(shoppingListItem);

            dialog.OkCommand.Subscribe(async x =>
            {
                var differece = x - shoppingListItem.Count;

                if (differece == 0)
                {
                    return;
                }

                if (differece > 0)
                {
                    await _orderEditService.IncreaseProductShoppingListItem(shoppingListItem.Source, differece);
                }
                else
                {
                    await _orderEditService.DecreaseProductShoppingListItem(shoppingListItem.Source, -differece);
                }

                shoppingListItem.Count = x;
            });

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();


            await dialog.WaitDialogClose();
        });
        _productService = productService;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        ShoppingList = new(_orderEditService);

        var productService = await Locator.GetInitializedService<IProductService>();
        var categoryService = await Locator.GetInitializedService<ICategoryService>();

        _orderEditService.BindSelectedCategoryItems<ProductHostItemVm>(x =>
        {
            if (x is ProductDto productDto)
            {
                return new ProductViewModel(_orderEditService, productService, productDto);
            }
            else
            {
                return new CategoryViewModel(categoryService.RuntimeCategories, (CategoryDto)x);
            }
        }, out var categoryItems)
                       .DisposeWith(disposables);

        _orderEditService.BindShoppingListItems((x, y) => new ProductShoppingListItemViewModel(x, y, _orderEditService), out var shoppingListItems)
                       .DisposeWith(disposables);

        _orderEditService.BindAdditivesForSelectedProduct(x => new AdditiveViewModel(x, _orderEditService, _additiveService), out var fastAdditives)
                       .DisposeWith(disposables);

        CurrentCategoryItems = categoryItems;
        ShoppingListItems = shoppingListItems;
        FastAdditives = fastAdditives;

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => ShoppingList.Subtotal = x)
                         .DisposeWith(disposables);
    }

    public bool IsMultiSelect
    {

        get => _orderEditService.IsMultiSelect.Value;
        set => _orderEditService.SetMultiSelect(value);
    }

    [Reactive]
    public ShoppingListViewModel? ShoppingList
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<AdditiveViewModel>? FastAdditives
    {
        get; private set;
    }

    [Reactive]
    public string? TotalComment
    {
        get; set;
    }

    [Reactive]
    public string? Category
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> CreateTotalCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SearchAddictiveCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreatePromocodeCommand
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

    public ReactiveCommand<Unit, Unit> SearchProductCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreateCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenMoreDialogCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDiscountsAndSurchargesDialog
    {
        get;
    }

    [Reactive]
    public IDiscountAccesser? DiscountAccesser
    {
        get; set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductHostItemVm>? CurrentCategoryItems
    {
        get; private set;
    }
    public ReactiveCommand<Unit, Unit> OpenQuantityVolumeDialogCommand
    {
        get;
    }

    [Reactive]
    public bool IsForHere
    {
        get; set;
    }
    public IOrderEditService OrderEditService
    {
        get;
    }
}