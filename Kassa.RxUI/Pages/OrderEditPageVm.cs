﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public class OrderEditPageVm : PageViewModel, IOrderEditVm
{
    private readonly IOrderEditService _order;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;

    public OrderEditPageVm(IOrderEditService order, ICashierService cashierService, IAdditiveService additiveService)
    {
        _order = order;
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
                await _order!.WriteTotalComment(dialog.Comment);
                TotalComment = dialog.Comment;
            }
        });

        CreatePromocodeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var promo = new PromocodeDialogViewModel(this);

            promo.ApplyCommand.Subscribe(x =>
            {
                DiscountAccesser = x;
            })
            ;
            var dialog = await MainViewModel.DialogOpenCommand.Execute(promo).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SearchAddictiveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var additiveService = await Locator.GetInitializedService<IAdditiveService>();
            var addictiveDialog = new SearchAddictiveDialogViewModel(additiveService, order);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SelectFavouriteCommand = ReactiveCommand.CreateFromTask(async (int x) =>
        {
            if (_order == null)
            {
                return;
            }
            await _order.SelectFavourite(x);
        });

        SelectRootCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (_order == null)
            {

                return;
            }
            await _order.SelectRootCategory();
        });

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var searchProductDialog = new SearchProductDialogViewModel();
            var dialog = await MainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished && _order is not null)
            {
                await _order.WriteCommentToSelectedItems(dialog.Comment);
            }
        });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new MoreCashierDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        OpenDiscountsAndSurchargesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new DiscountsAndSurchargesDialogViewModel();

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        GoToPaymentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var payment = await _cashierService.CreatePayment(order);
            var paymentPage = new CashierPaymentPageVm(payment);

            MainViewModel.GoToPageCommand.Execute(paymentPage).Subscribe();
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
                    await _order.IncreaseProductShoppingListItem(shoppingListItem.Source, differece);
                }
                else
                {
                    await _order.DecreaseProductShoppingListItem(shoppingListItem.Source, -differece);
                }

                shoppingListItem.Count = x;
            });

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();


            await dialog.WaitDialogClose();
        });

        GoToAllOrdersCommand = ReactiveCommand.CreateFromTask(() => {
            return Task.CompletedTask;
        });

        GoToAllDeliveriesPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allDeliveriesPage = new AllDeliveriesPageVm();

            await MainViewModel.GoToPage(allDeliveriesPage);
        });

        ShoppingList = new(_order);
        ShoppingList.DisposeWith(InternalDisposables);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await ShoppingList.InitializeAsync();

        var productService = await Locator.GetInitializedService<IProductService>();
        var categoryService = await Locator.GetInitializedService<ICategoryService>();

        _order.BindSelectedCategoryItems<ProductHostItemVm>(x =>
        {
            if (x is ProductDto productDto)
            {
                return new ProductViewModel(_order, productService,productDto);
            }
            else 
            {
                 return new CategoryViewModel(categoryService.RuntimeCategories, (CategoryDto)x);
            }
        }, out var categoryItems)
                       .DisposeWith(disposables);

        _order.BindShoppingListItems((x, y) => new ProductShoppingListItemViewModel(x, y, _order), out var shoppingListItems)
                       .DisposeWith(disposables);

        _order.BindAdditivesForSelectedProduct(x => new AdditiveViewModel(x, _order, _additiveService), out var fastAdditives)
                       .DisposeWith(disposables);

        CurrentCategoryItems = categoryItems;
        ShoppingListItems = shoppingListItems;
        FastAdditives = fastAdditives;

        this.WhenAnyValue(x => x.DiscountAccesser)
            .Subscribe(x =>
            {
                foreach (var item in ShoppingList.AddictiveViewModels)
                {
                    if (x is IDiscountAccesser discountAccesser)
                    {
                        item.HasDiscount = true;
                        if (double.IsNaN(discountAccesser.AccessDicsount(item.ItemId)))
                        {
                            item.HasDiscount = false;
                            item.Discount = 0;
                        }
                        else
                        {
                            item.Discount = discountAccesser.AccessDicsount(item.ItemId);
                        }
                        continue;
                    }

                    item.HasDiscount = false;
                    item.Discount = 0;
                }


            })
            .DisposeWith(disposables);

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => ShoppingList.Subtotal = x)
                         .DisposeWith(disposables);
    }

    public ShoppingListViewModel ShoppingList
    {
        get;
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

    public ReactiveCommand<Unit, Unit> GoToPaymentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoToAllOrdersCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoToAllDeliveriesPageCommand
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
    ReadOnlyObservableCollection<ICategoryItemDto>? IOrderEditVm.CurrentCategoryItems
    {
        get;
    }
}
