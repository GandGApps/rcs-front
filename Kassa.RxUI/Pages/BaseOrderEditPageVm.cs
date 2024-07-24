using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using System.Reactive.Linq;
using DynamicData.Binding;
using DynamicData;

namespace Kassa.RxUI.Pages;
public abstract class BaseOrderEditPageVm: BaseViewModel, IOrderEditVm
{
    private readonly OrderEditDto _orderEditDto;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;
    private readonly IProductService _productService;
    private readonly IStorageScope _storageScope;

    public BaseOrderEditPageVm(OrderEditDto orderEditDto, IStorageScope storageScope, ICashierService cashierService, IAdditiveService additiveService, IProductService productService)
    {
        _orderEditDto = orderEditDto;
        _cashierService = cashierService;
        _additiveService = additiveService;
        _productService = productService;
        _storageScope = storageScope;

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
            var addictiveDialog = new SearchAddictiveDialogViewModel(additiveService, orderEditDto);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SelectFavouriteCommand = ReactiveCommand.CreateFromTask(async (int x) =>
        {
            if (_orderEditDto == null)
            {
                return;
            }
            await _orderEditDto.SelectFavourite(x);
        });

        SelectRootCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (_orderEditDto == null)
            {

                return;
            }
            await _orderEditDto.SelectRootCategory();
        });

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var searchProductDialog = new SearchProductDialogViewModel(_orderEditDto, _productService);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished && _orderEditDto is not null)
            {
                await _orderEditDto.WriteCommentToSelectedItems(dialog.Comment);
            }
        });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new MoreCashierDialogViewModel(this, _orderEditDto);

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
            var payment = await _cashierService.CreatePayment(orderEditDto);
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
                    await _orderEditDto.IncreaseProductShoppingListItem(shoppingListItem.Source, differece);
                }
                else
                {
                    await _orderEditDto.DecreaseProductShoppingListItem(shoppingListItem.Source, -differece);
                }

                shoppingListItem.Count = x;
            });

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();


            await dialog.WaitDialogClose();
        });

        GoToAllOrdersCommand = ReactiveCommand.CreateFromTask(() =>
        {
            return Task.CompletedTask;
        });

        GoToAllDeliveriesPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allDeliveriesPage = new AllDeliveriesPageVm();

            await MainViewModel.GoToPage(allDeliveriesPage);
        });

        ForHereOrToGoCommand = ReactiveCommand.Create(() =>
        {
            IsForHere = !IsForHere;
            _orderEditDto.SetIsForHere(IsForHere);
        });

        OpenPortionDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var firstSelected = ShoppingListItems?.FirstOrDefault(x => x.IsSelected);

            if (firstSelected == null)
            {
                return;
            }

            var dialog = new PortionDialogVm(orderEditDto, firstSelected);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        ShoppingList = new(_orderEditDto);
        ShoppingList.DisposeWith(InternalDisposables);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        WhenOrderStarted = DateTime.Now;

        await ShoppingList.InitializeAsync();

        var productService = await Locator.GetInitializedService<IProductService>();
        var categoryService = await Locator.GetInitializedService<ICategoryService>();

        CurrentCategoryItems = categoryItems;
        ShoppingListItems = shoppingListItems;
        FastAdditives = fastAdditives;

        ShoppingList.ProductShoppingListItems
                         .ToObservableChangeSet()
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

    public IStorageScope StorageScope => _storageScope;


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

    public ReactiveCommand<Unit, Unit> ForHereOrToGoCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenPortionDialogCommand
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
        get; private set;
    }

    [Reactive]
    public DateTime WhenOrderStarted
    {
        get; set;
    }
}
