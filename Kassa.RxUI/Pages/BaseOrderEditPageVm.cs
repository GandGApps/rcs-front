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
using Splat;
using Kassa.Shared;

namespace Kassa.RxUI.Pages;
public abstract class BaseOrderEditPageVm : PageViewModel, IOrderEditVm
{
    private readonly OrderEditDto _orderEditDto;
    private readonly ICashierService _cashierService;
    private readonly IAdditiveService _additiveService;
    private readonly IProductService _productService;
    private readonly IStorageScope _storageScope;
    private readonly ICategoryService _categoryService;
    private readonly IReceiptService _receiptService;
    private readonly IIngridientsService _ingridientsService;

    private readonly ObservableCollection<ProductHostItemVm> _currentCategoryItems = [];

    protected readonly List<HostHistory> _hostNavigationStack = [];

    public BaseOrderEditPageVm(
        OrderEditDto orderEditDto,
        IStorageScope storageScope,
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        ICategoryService categoryService,
        IReceiptService receiptService,
        IIngridientsService ingridientsService)
    {
        _orderEditDto = orderEditDto;
        _cashierService = cashierService;
        _additiveService = additiveService;
        _productService = productService;
        _storageScope = storageScope;
        _categoryService = categoryService;
        _receiptService = receiptService;
        _ingridientsService = ingridientsService;

        CurrentHostedItems = new(_currentCategoryItems);

        ShoppingList = new(this, _productService, _receiptService, _additiveService);

        foreach(var productShoppingListItem in orderEditDto.Products)
        {

            if(!_productService.RuntimeProducts.TryGetValue(productShoppingListItem.ItemId, out var productDto))
            {
                this.Log().Error($"Product with id {productShoppingListItem.ItemId} not found");
                continue;
            }

            var productVm = new ProductShoppingListItemViewModel(productShoppingListItem, productDto, _productService.RuntimeProducts, this, _receiptService, _additiveService);

            ShoppingList.AddProductShoppingListItemUnsafe(productVm);
        }

        ShoppingList.DisposeWith(InternalDisposables);

        ShoppingList.ProductShoppingListItems
            .ToObservableChangeSet()
            .AutoRefresh(x => x.SubtotalSum)
            .ToCollection()
            .Select(x => x.Sum(x => x.SubtotalSum))
            .Subscribe(x => ShoppingList.Subtotal = x)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsOutOfTurn)
            .BindTo(orderEditDto, x => x.IsOutOfTurn)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.TotalComment)
            .BindTo(orderEditDto, x => x.Comment)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsForHere)
            .BindTo(orderEditDto, x => x.IsForHere)
            .DisposeWith(InternalDisposables);


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
                _orderEditDto.Comment = dialog.Comment;
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
            var addictiveDialog = new SearchAddictiveDialogViewModel(_additiveService, this);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SelectFavouriteCommand = ReactiveCommand.Create((int x) =>
        {
            if (_orderEditDto == null)
            {
                return;
            }

            _currentCategoryItems.Clear();

            // Unsafe method, because it doesn't use repository
            // Maybe it should be refactored to use repository

            var favouriteCategories = _categoryService.RuntimeCategories.Values
                .Where(category => category.Favourites.Contains(x))
                .Select(dto => new CategoryViewModel(_categoryService.RuntimeCategories, dto, this));

            var favouriteProducts = _productService.RuntimeProducts.Values
                .Where(product => product.Favourites.Contains(x))
                .Select(dto => new ProductViewModel(this, _productService, dto));

            _currentCategoryItems.AddRange(favouriteCategories);
            _currentCategoryItems.AddRange(favouriteProducts);

            var array = new ProductHostItemVm[_currentCategoryItems.Count];
            _currentCategoryItems.CopyTo(array, 0);

            _hostNavigationStack.Add(HostHistory.CreateFavourite(x, array));

            CurrentFavourite = x;
        });

        SelectRootCategoryCommand = ReactiveCommand.Create(() =>
        {
            _currentCategoryItems.Clear();

            var rootCategories = _categoryService.RuntimeCategories.Values
                .Where(category => category.CategoryId == null)
                .Select(dto => new CategoryViewModel(_categoryService.RuntimeCategories, dto, this));

            var rootProducts = _productService.RuntimeProducts.Values
                .Where(product => product.CategoryId == null)
                .Select(dto => new ProductViewModel(this, _productService, dto));

            _currentCategoryItems.AddRange(rootCategories);
            _currentCategoryItems.AddRange(rootProducts);

            _hostNavigationStack.Clear();

            var array = new ProductHostItemVm[_currentCategoryItems.Count];
            _currentCategoryItems.CopyTo(array, 0);

            _hostNavigationStack.Add(HostHistory.CreateRoot(array));

            CurrentCategory = null;
        });

        SelectRootCategoryCommand.Execute().Subscribe();

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var searchProductDialog = new SearchProductDialogViewModel(this, _productService);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await MainViewModel.ShowDialogAndWaitClose(dialog);

            if (dialog.IsPublished)
            {
                foreach (var item in ShoppingList.SelectedItems)
                {
                    if (item is ProductShoppingListItemViewModel product)
                    {
                        product.Comment = dialog.Comment;
                    }
                }
            }
        });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new MoreCashierDialogViewModel(this);

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        OpenDiscountsAndSurchargesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new DiscountsAndSurchargesDialogViewModel();

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        GoToPaymentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            PrepateOrderEdit();

            var payment = await _cashierService.CreatePayment(orderEditDto);
            var paymentPage = new CashierPaymentPageVm(ShoppingList.ProductShoppingListItems, this, payment, cashierService, additiveService, productService, ingridientsService, receiptService, categoryService);

            MainViewModel.GoToPageCommand.Execute(paymentPage).Subscribe();
        });

        OpenQuantityVolumeDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var shoppingListItem = ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>().FirstOrDefault();

            if (shoppingListItem == null)
            {
                return;
            }

            var dialog = new QuantityVolumeDialogVewModel(shoppingListItem);

            dialog.OkCommand.Subscribe(async x =>
            {
                var difference = x - shoppingListItem.Count;

                if (difference == 0)
                {
                    return;
                }

                if (difference > 0)
                {
                    await ShoppingList.IncreaseProductShoppingListItemViewModel(shoppingListItem, difference);
                }
                else
                {
                    await ShoppingList.DecreaseProductShoppingListItemViewModel(shoppingListItem, -difference);
                }

                shoppingListItem.Count = x;
            });

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        GoToAllOrdersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var shiftService = RcsKassa.GetRequiredService<IShiftService>();  
            var ordersServcice = RcsKassa.GetRequiredService<IOrdersService>();

            var servicePage = new ServicePageVm(_cashierService, shiftService, ordersServcice, _productService);

            await MainViewModel.GoToPage(servicePage);
        });

        GoToAllDeliveriesPageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allDeliveriesPage = RcsKassa.CreateAndInject<AllDeliveriesPageVm>();

            await MainViewModel.GoToPage(allDeliveriesPage);
        });

        ForHereOrToGoCommand = ReactiveCommand.Create(() =>
        {
            IsForHere = !IsForHere;
        });

        OpenPortionDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var firstSelected = ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>().FirstOrDefault();

            if (firstSelected == null)
            {
                return;
            }

            var dialog = new PortionDialogVm(this, _receiptService, firstSelected);

            await MainViewModel.ShowDialogAndWaitClose(dialog);
        });

        NavigateBackCategoryCommand = ReactiveCommand.Create(() =>
        {
            var last = _hostNavigationStack.Last();

            if (last.IsRoot)
            {
                return;
            }

            last.Dispose();

            _hostNavigationStack.RemoveAt(_hostNavigationStack.Count - 1);

            last = _hostNavigationStack.Last();

            _currentCategoryItems.Clear();
            _currentCategoryItems.AddRange(last.Items);

            CurrentFavourite = last.Favourite;
            CurrentCategory = last.IsCategory ? _categoryService.RuntimeCategories[last.CategoryId] : null;
        });
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

    public Guid OrderId => _orderEditDto.Id;

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

    public ReactiveCommand<Unit, Unit> NavigateBackCategoryCommand
    {
        get;
    }

    [Reactive]
    public IDiscountAccesser? DiscountAccesser
    {
        get; set;
    }

    public ReadOnlyObservableCollection<ProductHostItemVm>? CurrentHostedItems
    {
        get;
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

    [Reactive]
    public DateTime WhenOrderStarted
    {
        get; set;
    }

    [Reactive]
    public CategoryDto? CurrentCategory
    {
        get; private set;
    }

    [Reactive]
    public int CurrentFavourite
    {
        get; private set;
    }

    [Reactive]
    public bool IsStopList
    {
        get; set;
    }

    [Reactive]
    public bool IsShowPrice
    {
        get; set;
    }

    [Reactive]
    public bool IsOutOfTurn
    {
        get; set;
    }

    public void MoveToCategoryUnsafe(Guid id)
    {
        _currentCategoryItems.Clear();

        var categories = _categoryService.RuntimeCategories.Values
            .Where(category => category.CategoryId == id)
            .Select(dto => new CategoryViewModel(_categoryService.RuntimeCategories, dto, this));

        var products = _productService.RuntimeProducts.Values
            .Where(product => product.CategoryId == id)
            .Select(dto => new ProductViewModel(this, _productService, dto));

        _currentCategoryItems.AddRange(categories);
        _currentCategoryItems.AddRange(products);

        var array = new ProductHostItemVm[_currentCategoryItems.Count];
        _currentCategoryItems.CopyTo(array, 0);

        _hostNavigationStack.Add(HostHistory.CreateCategory(id, array));

    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        WhenOrderStarted = DateTime.Now;

        await ShoppingList.InitializeAsync();
    }

    protected internal override ValueTask OnPageLeaving(PageViewModel? nextPage)
    {
        if(nextPage is MainPageVm)
        {
            PrepateOrderEdit();

            
        }

        return ValueTask.CompletedTask;

    }

    private void PrepateOrderEdit()
    {
        _orderEditDto.Products.Clear();

        foreach (var item in ShoppingList.ProductShoppingListItems)
        {
            // TODO: Use object pool
            var productItem = new ProductShoppingListItemDto(item.ProductDto)
            {
                Count = item.Count,
                Comment = item.Comment,
                SubtotalSum = item.SubtotalSum,
                Price = item.Price,
                Discount = item.Discount,
                HasDiscount = item.HasDiscount,
            };

            foreach (var additive in item.Additives)
            {
                // TODO: Use object pool
                var additiveItem = new AdditiveShoppingListItemDto(productItem, additive.AdditiveDto)
                {
                    Count = additive.Count,
                    Price = additive.Price,
                    Discount = additive.Discount,
                    HasDiscount = additive.HasDiscount,
                    SubtotalSum = additive.SubtotalSum,
                };

                productItem.Additives.Add(additiveItem);
            }

            _orderEditDto.Products.Add(productItem);
        }
    }

    protected readonly struct HostHistory(Guid categoryId, int favourite, ProductHostItemVm[] items)
    {
        public static HostHistory CreateFavourite(int favourite, ProductHostItemVm[] items) => new(Guid.Empty, favourite, items);
        public static HostHistory CreateRoot(ProductHostItemVm[] items) => new(Guid.Empty, -1, items);
        public static HostHistory CreateCategory(Guid categoryId, ProductHostItemVm[] items) => new(categoryId, -1, items);

        public static readonly HostHistory Empty = new(Guid.Empty, -1, []);

        public readonly Guid CategoryId => categoryId;
        public readonly int Favourite => favourite;
        public readonly ProductHostItemVm[] Items => items;

        public readonly bool IsRoot => categoryId == Guid.Empty && Favourite == -1;
        public readonly bool IsFavourite => categoryId == Guid.Empty && Favourite != -1;
        public readonly bool IsCategory => CategoryId != Guid.Empty;
        public readonly bool IsEmpty => items.Length == 0 && categoryId == Guid.Empty && Favourite == -1;


        public readonly void Dispose()
        {
            foreach (var item in items)
            {
                item.Dispose();
            }
        }
    }
}
