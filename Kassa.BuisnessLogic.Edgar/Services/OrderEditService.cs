using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using ReactiveUI;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class OrderEditService : BaseInitializableService, IOrderEditService
{

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IAdditiveService _additiveService;
    private readonly IReceiptService _receiptService;

    private readonly BehaviorSubject<bool> _isMultiSelectObservable = new(false);
    private readonly ObservableOnlyBehaviourSubject<bool> _isMultiSelect;

    private readonly BehaviorSubject<ICategoryDto?> _currentCategoryObservable = new(null);
    private readonly ObservableOnlyBehaviourSubject<ICategoryDto?> _currentCategory;

    private readonly BehaviorSubject<int?> _selectedFavoriteObservable = new(null);
    private readonly ObservableOnlyBehaviourSubject<int?> _selectedFavorite;

    private readonly BehaviorSubject<string?> _totalCommentObservable = new(null);
    private readonly ObservableOnlyBehaviourSubject<string?> _totalComment;

    private readonly BehaviorSubject<bool> _showPriceObservable = new(false);
    private readonly ObservableOnlyBehaviourSubject<bool> _showPrice;

    private readonly BehaviorSubject<double> _discountObservable = new(0);
    private readonly ObservableOnlyBehaviourSubject<double> _discount;

    private OrderDto? _order;

    private readonly List<ICategoryDto> _categoriesStack = [];
    private readonly Dictionary<Guid, IApplicationModelManager<AdditiveShoppingListItemDto>> _additivesInProductDto = [];

    public OrderEditService(
        IProductService productService,
        ICategoryService categoryService,
        IAdditiveService additiveService,
        IReceiptService receiptService,
        OrderDto? order)
    {
        _productService = productService;
        _categoryService = categoryService;
        _additiveService = additiveService;
        _receiptService = receiptService;
        _order = order;

        _isMultiSelect = new(_isMultiSelectObservable);
        _currentCategory = new(_currentCategoryObservable);
        _selectedFavorite = new(_selectedFavoriteObservable);
        _totalComment = new(_totalCommentObservable);
        _showPrice = new(_showPriceObservable);
        _discount = new(_discountObservable);
    }



    public IReadOnlyList<ICategoryDto> CategoriesStack => _categoriesStack;

    public IObservableOnlyBehaviourSubject<ICategoryDto?> CurrentCategory => _currentCategory;
    public IObservableOnlyBehaviourSubject<int?> SelectedFavourite => _selectedFavorite;
    public IObservableOnlyBehaviourSubject<bool> IsMultiSelect => _isMultiSelect;
    public IObservableOnlyBehaviourSubject<string?> TotalComment => _totalComment;
    public IObservableOnlyBehaviourSubject<bool> ShowPrice => _showPrice;
    public IObservableOnlyBehaviourSubject<double> Discount => _discount;

    public IApplicationModelManager<ProductShoppingListItemDto> ShoppingListItems
    {
        get;
    } = new HostModelManager<ProductShoppingListItemDto>();

    public IApplicationModelManager<IShoppingListItemDto> SelectedShoppingListItems
    {
        get;
    } = new HostModelManager<IShoppingListItemDto>();


    public Guid OrderId
    {
        get; set;
    }


    public bool IsDelivery
    {
        get; set;
    }

    public IDisposable BindSelectedCategoryItems<T>(Func<ICategoryItemDto, T> creator, out ReadOnlyObservableCollection<T> categoryItems) where T : class, IModel
    {
        this.ThrowIfNotInitialized();

        var target = new ObservableCollection<T>();
        var ordered = new ObservableCollection<T>();
        categoryItems = new(ordered);

        IDisposable? observeCategories = null;
        IDisposable? observeproducts = null;

        //TODO: create

        var disposable = CurrentCategory
            .Subscribe(x =>
            {
                target.Clear();

                observeCategories?.Dispose();
                observeproducts?.Dispose();

                if (x is FavouriteCategoryDto favouriteCategory)
                {
                    target.AddRange(_productService.RuntimeProducts.Values
                        .Cast<ICategoryItemDto>()
                        .Where(x => x.Favourites.Contains(favouriteCategory.Favourite))
                        .Select(x => creator(x)));

                    observeproducts = _productService.RuntimeProducts
                        .Filter(p => p.Favourites.Contains(favouriteCategory.Favourite))
                        .Subscribe(changeSet =>
                        {
                            if (changeSet.IsEmpty)
                            {
                                return;
                            }

                            foreach (var change in changeSet.Changes)
                            {

                                switch (change.Reason)
                                {

                                    case ModelChangeReason.Add:
                                        target.Add(creator(change.Current));
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(creator(change.Current));
                                        break;

                                    case ModelChangeReason.Refresh:
                                        target.Remove(change.Previous is null ? creator(change.Current) : creator(change.Previous));
                                        target.Add(creator(change.Current));
                                        break;
                                }
                            }
                        });

                    observeCategories = _categoryService.RuntimeCategories.BindAndFilter(c => c.Favourites.Contains(favouriteCategory.Favourite), x => creator(x), target);

                    ordered.Clear();
                    ordered.AddRange(target.OrderBy(x =>
                    {
                        if (x is ProductDto)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }));
                }

                if (x is CategoryDto category)
                {
                    target.AddRange(_categoryService.RuntimeCategories.Values
                                               .Where(c => c.CategoryId == category.Id)
                                               .Select(x => creator(x)));

                    observeCategories = _categoryService.RuntimeCategories.BindAndFilter(c => c.CategoryId == category.Id, x => creator(x), target);

                    target.AddRange(_productService.RuntimeProducts.Values
                            .Cast<ICategoryItemDto>()
                            .Where(p => p.CategoryId == category.Id)
                            .Select(x => creator(x)));

                    observeproducts = _productService.RuntimeProducts
                        .Filter(p => p.CategoryId == category.Id)
                        .Subscribe(changeSet =>
                        {
                            if (changeSet.IsEmpty)
                            {
                                return;
                            }

                            foreach (var change in changeSet.Changes)
                            {

                                switch (change.Reason)
                                {
                                    case ModelChangeReason.Add:
                                        target.Add(creator(change.Current));
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(creator(change.Current));
                                        break;
                                }
                            }
                        });

                    ordered.Clear();
                    ordered.AddRange(target.OrderBy(x =>
                    {
                        if (x is ProductDto)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }));
                }

                if (x is null)
                {
                    observeCategories = _categoryService.RuntimeCategories.BindAndFilter(c => c.CategoryId == null, x => creator(x), target);

                    target.AddRange(_productService.RuntimeProducts.Values
                        .Cast<ICategoryItemDto>()
                        .Where(p => p.CategoryId == null)
                        .Select(x => creator(x)));

                    observeproducts = _productService.RuntimeProducts
                        .Filter(p => p.CategoryId == null)
                        .Subscribe(changeSet =>
                        {

                            if (changeSet.IsEmpty)
                            {

                                return;
                            }

                            foreach (var change in changeSet.Changes)
                            {


                                switch (change.Reason)
                                {


                                    case ModelChangeReason.Add:
                                        target.Add(creator(change.Current));
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(creator(change.Current));
                                        break;

                                    case ModelChangeReason.Refresh:
                                        target.Remove(change.Previous is null ? creator(change.Current) : creator(change.Previous));
                                        target.Add(creator(change.Current));
                                        break;
                                }
                            }
                        });

                    ordered.Clear();
                    ordered.AddRange(target.OrderBy(x =>
                    {

                        if (x is ProductDto)
                        {

                            return 1;
                        }
                        else
                        {

                            return 0;
                        }
                    }));
                }


            });

        return Disposable.Create(() =>
        {
            disposable.Dispose();
            observeCategories?.Dispose();
            observeproducts?.Dispose();

            target = null;
            ordered = null;
        });
    }

    public IDisposable BindShoppingListItems<T>(Func<ProductShoppingListItemDto, IApplicationModelManager<ProductShoppingListItemDto>, T> creator, out ReadOnlyObservableCollection<T> shoppingListItems) where T : class, IModel
    {
        this.ThrowIfNotInitialized();

        var observableCollection = new ObservableCollection<T>();
        shoppingListItems = new(observableCollection);

        observableCollection.AddRange(ShoppingListItems.Values.Select(x =>
        {
            var result = creator(x, ShoppingListItems);
            return result;
        }));


        return ShoppingListItems.Subscribe(async x =>
        {
            foreach (var change in x.Changes)
            {
                switch (change.Reason)
                {
                    case ModelChangeReason.Add:
                        var vm = creator(change.Current, ShoppingListItems);
                        observableCollection.Insert(0, vm);
                        await SelectShoppingListItem(change.Current);
                        break;

                    case ModelChangeReason.Remove:
                        observableCollection.Remove(observableCollection.First(x => x.Id == change.Current.Id));
                        break;
                }
            }
        });
    }

    public IDisposable BindSelectedShoppingListItems(out ReadOnlyObservableCollection<IShoppingListItemDto> shoppingListItems)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Attention, additives not will update at runtime, that's why you need update it manually.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="creator"></param>
    /// <param name="additives"></param>
    /// <returns></returns>
    public IDisposable BindAdditivesForSelectedProduct<T>(Func<AdditiveDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<Guid, AdditiveDto>
    {
        this.ThrowIfNotInitialized();

        var disposables = new CompositeDisposable();
        var internalDisposables = new CompositeDisposable();

        var observableCollection = new ObservableCollection<T>();
        additives = new(observableCollection);

        SelectedShoppingListItems
            .Subscribe(changeSet =>
            {
                foreach (var change in changeSet.Changes)
                {
                    if (change.Current is not ProductShoppingListItemDto product)
                    {
                        continue;
                    }

                    switch (change.Reason)
                    {
                        case ModelChangeReason.Add:
                            observableCollection.AddRange(_additiveService.RuntimeAdditives.Values
                                .Where(x => x.ProductIds.Contains(change.Current.ItemId))
                                .Select(x => creator(x)));
                            break;

                        case ModelChangeReason.Remove:
                            observableCollection.RemoveMany(observableCollection.Where(x => x.Source.ProductIds.Contains(change.Current.ItemId)));
                            break;
                    }
                }
            })
            .DisposeWith(disposables);



        return disposables;
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var item in _additivesInProductDto)
        {
            item.Value.Dispose();
        }
    }

    public ValueTask Initialize()
    {
        if (IsInitialized)
        {
            return new ValueTask();
        }

        if (_order is null)
        {
            OrderId = Guid.NewGuid();
        }
        else
        {
            OrderId = _order.Id;
        }

        IsInitialized = true;

        return ValueTask.CompletedTask;
    }

    public async Task SelectCategory(Guid categoryId)
    {
        this.ThrowIfNotInitialized();

        var category = await _categoryService.GetCategoryById(categoryId);

        if (category is null)
        {
            throw new InvalidOperationException($"Category with id {categoryId} not found.");
        }

        _categoriesStack.Add(category);
        _currentCategoryObservable.OnNext(category);
    }
    public ValueTask<bool> SelectPreviosCategory()
    {
        this.ThrowIfNotInitialized();

        if (_categoriesStack.Count == 0)
        {
            return new(false);
        }

        _categoriesStack.RemoveAt(_categoriesStack.Count - 1);

        if (_categoriesStack.Count == 0)
        {
            _currentCategoryObservable.OnNext(null);
        }
        else
        {
            _currentCategoryObservable.OnNext(_categoriesStack[^1]);
        }

        return new(true);
    }

    public ValueTask SelectFavourite(int favourite)
    {
        this.ThrowIfNotInitialized();

        _categoriesStack.Clear();

        _currentCategoryObservable.OnNext(new FavouriteCategoryDto(favourite));
        _selectedFavoriteObservable.OnNext(favourite);


        return ValueTask.CompletedTask;
    }

    public ValueTask SelectRootCategory()
    {
        this.ThrowIfNotInitialized();

        _currentCategoryObservable.OnNext(null);

        _selectedFavoriteObservable.OnNext(null);
        _categoriesStack.Clear();

        return ValueTask.CompletedTask;
    }

    public async Task AddProductToShoppingList(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await _productService.GetProductById(productId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found.");
        }

        var receipt = await _receiptService.GetReceipt(product.ReceiptId);

        if (receipt is not null)
        {
            if (!await _receiptService.HasEnoughIngridients(receipt))
            {
                throw new InvalidOperationException($"Not enough ingredients for product {product.Name}");
            }
        }

        await _productService.DecreaseProductCount(product.Id);

        product = (await _productService.GetProductById(productId))!;

        var existItem = ShoppingListItems.Values.FirstOrDefault(x => x.ItemId == product.Id);

        if (existItem != null)
        {
            existItem.Count++;
            ShoppingListItems.AddOrUpdate(existItem);

            product.IsAdded = true;
            _productService.RuntimeProducts.AddOrUpdate(product);

            return;
        }

        product.IsAdded = true;
        _productService.RuntimeProducts.AddOrUpdate(product);

        var shoppingListItem = new ProductShoppingListItemDto(product)
        {
            Id = Guid.NewGuid()
        };

        ShoppingListItems.AddOrUpdate(shoppingListItem);
    }

    public async Task AddProductToShoppingList(OrderedProductDto orderedProduct)
    {
        this.ThrowIfNotInitialized();

        var product = await _productService.GetProductById(orderedProduct.ProductId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {orderedProduct.ProductId} not found.");
        }

        var shoppingListItem = new ProductShoppingListItemDto(orderedProduct, product);

        ShoppingListItems.AddOrUpdate(shoppingListItem);

        var sourceCache = new HostModelManager<AdditiveShoppingListItemDto>();

        foreach (var orderedAdditive in orderedProduct.Additives)
        {

            var additive = await _additiveService.GetAdditiveById(orderedAdditive.AdditiveId);

            if (additive is null)
            {
                throw new InvalidOperationException($"Additive with id {orderedAdditive.AdditiveId} not found.");
            }

            var additiveShoppingListItem = new AdditiveShoppingListItemDto(orderedAdditive, shoppingListItem, additive);

            sourceCache.AddOrUpdate(additiveShoppingListItem);
        }

        Debug.Assert(!_additivesInProductDto.ContainsKey(product.Id));

        _additivesInProductDto.Add(product.Id, sourceCache);
    }

    public Task SelectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {
            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }

        if (!IsMultiSelect.Value)
        {
            ClearSelectedShoppingListItems();
        }

        SelectedShoppingListItems.AddOrUpdate(shoppingListItemDto);

        if (shoppingListItemDto is ProductShoppingListItemDto product)
        {
            product.IsSelected = true;

            ShoppingListItems.AddOrUpdate(product);
            SelectedShoppingListItems.AddOrUpdate(product);
        }

        if (shoppingListItemDto is AdditiveShoppingListItemDto additive)
        {
            if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
            {
                additive.IsSelected = true;
                sourceCache.AddOrUpdate(additive);
                SelectedShoppingListItems.AddOrUpdate(additive);
            }
        }

        return Task.CompletedTask;
    }

    public async Task RemoveShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {
            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }

        if (shoppingListItemDto is ProductShoppingListItemDto shoppingListItem)
        {
            var product = await _productService.GetProductById(shoppingListItem.ItemId);
            if (product != null)
            {
                product.IsAdded = false;

                await _productService.IncreaseProductCount(product.Id);
                _productService.RuntimeProducts.AddOrUpdate(product);
            }

            SelectedShoppingListItems.Remove(shoppingListItemDto.Id);
        }
    }

    public Task UnselectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {
            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }

        SelectedShoppingListItems.Remove(shoppingListItemDto.Id);

        if (shoppingListItemDto is ProductShoppingListItemDto shoppingListItem)
        {
            shoppingListItem.IsSelected = false;
            ShoppingListItems.AddOrUpdate(shoppingListItem);
        }

        if (shoppingListItemDto is AdditiveShoppingListItemDto additive)
        {

            if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
            {
                additive.IsSelected = false;
                sourceCache.AddOrUpdate(additive);
            }
        }

        return Task.CompletedTask;
    }

    private void ClearSelectedShoppingListItems()
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {

            if (shoppingListItem is ProductShoppingListItemDto shoppingListItemDto)
            {
                shoppingListItemDto.IsSelected = false;
                ShoppingListItems.AddOrUpdate(shoppingListItemDto);
            }

            if (shoppingListItem is AdditiveShoppingListItemDto additive)
            {

                if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
                {
                    additive.IsSelected = false;
                    sourceCache.AddOrUpdate(additive);
                }
            }
        }
        SelectedShoppingListItems.Clear();
    }

    public async Task AddAdditiveToSelectedProducts(Guid additiveId)
    {

        this.ThrowIfNotInitialized();

        var additive = await _additiveService.GetAdditiveById(additiveId);

        if (additive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveId} not found.");
        }

        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {
            if (additive.ProductIds.Contains(shoppingListItem.ItemId) &&
                shoppingListItem is ProductShoppingListItemDto product)
            {
                await _additiveService.DecreaseAddtiveCount(additive);

                var updatedAdditive = new AdditiveShoppingListItemDto(product, additive)
                {
                    Id = Guid.NewGuid()
                };

                if (_additivesInProductDto.TryGetValue(product.Id, out var sourceCache))
                {
                    sourceCache.AddOrUpdate(updatedAdditive);
                }
            }
        }

    }

    public bool IsAdditiveAdded(Guid additiveId)
    {
        this.ThrowIfNotInitialized();

        return SelectedShoppingListItems.Values.Select(x => x.Id)
            .Where(_additivesInProductDto.ContainsKey)
            .Select(id => _additivesInProductDto[id].Values)
            .Any(x => x.Any(additive => additive.ItemId == additiveId));
    }

    public IDisposable BindAdditivesForProductShoppingListItem<T>(ProductShoppingListItemDto item, Func<AdditiveShoppingListItemDto, IApplicationModelManager<AdditiveShoppingListItemDto>, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IApplicationModelPresenter<AdditiveShoppingListItemDto>
    {
        this.ThrowIfNotInitialized();

        var target = new ObservableCollection<T>();
        additives = new(target);

        if (_additivesInProductDto.TryGetValue(item.Id, out var sourceCache))
        {
            target.AddRange(sourceCache.Values.Select(x =>
            {
                var result = creator(x, sourceCache);
                return result;
            }));

            return sourceCache.Subscribe(x =>
            {
                foreach (var change in x.Changes)
                {
                    switch (change.Reason)
                    {

                        case ModelChangeReason.Add:
                            target.Add(creator(change.Current, sourceCache));
                            break;

                        case ModelChangeReason.Remove:
                            target.Remove(target.First(x => x.Id == change.Current.Id));
                            break;
                    }
                }
            });
        }

        sourceCache = new HostModelManager<AdditiveShoppingListItemDto>();

        _additivesInProductDto.Add(item.Id, sourceCache);


        target.AddRange(sourceCache.Values.Select(x =>
        {
            var result = creator(x, sourceCache);
            return result;
        }));

        return sourceCache.Subscribe(x =>
        {
            foreach (var change in x.Changes)
            {
                switch (change.Reason)
                {

                    case ModelChangeReason.Add:
                        target.Add(creator(change.Current, sourceCache));
                        break;

                    case ModelChangeReason.Remove:
                        target.Remove(target.First(x => x.Id == change.Current.Id));
                        break;
                }
            }
        });
    }

    public Task WriteCommentToSelectedItems(string? comment)
    {
        this.ThrowIfNotInitialized();


        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {

            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                product.AdditiveInfo = comment;
                ShoppingListItems.AddOrUpdate(product);
                SelectedShoppingListItems.AddOrUpdate(product);
            }
        }

        return Task.CompletedTask;
    }

    public Task WriteTotalComment(string? totalComment)
    {
        _totalCommentObservable.OnNext(totalComment);

        return Task.CompletedTask;
    }

    public async Task IncreaseSelectedProductShoppingListItem(double count)
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {
            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                await IncreaseProductShoppingListItem(product, count);
            }
        }
    }

    public async Task DecreaseSelectedProductShoppingListItem(double count)
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {

            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                await DecreaseProductShoppingListItem(product, count);
            }
        }
    }

    public async Task RemoveSelectedProductShoppingListItem()
    {
        this.ThrowIfNotInitialized();
        var needRemove = SelectedShoppingListItems.Values.ToList();

        foreach (var shoppingListItem in needRemove)
        {
            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                await RemoveProductShoppingListItem(product);
            }

            if (shoppingListItem is AdditiveShoppingListItemDto additive)
            {
                var additiveDto = await _additiveService.GetAdditiveById(additive.ItemId);

                if (additiveDto is null)
                {
                    throw new InvalidOperationException($"Additive with id {additive.ItemId} not found.");
                }

                await _additiveService.IncreaseAdditiveCount(additiveDto);

                if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
                {
                    sourceCache.Remove(additive.Id);
                }
                SelectedShoppingListItems.Remove(additive.Id);
            }
        }
    }

    public async Task IncreaseProductShoppingListItem(ProductShoppingListItemDto item, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var product = await _productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        var receipt = await _receiptService.GetReceipt(product.ReceiptId);

        if (receipt is not null)
        {
            if (!await _receiptService.HasEnoughIngridients(receipt))
            {
                throw new InvalidOperationException($"Not enough ingredients for product {product.Name}");
            }

        }


        await _productService.DecreaseProductCount(product.Id, count);
        item.Count += count;
        ShoppingListItems.AddOrUpdate(item);
        SelectedShoppingListItems.AddOrUpdate(item);
    }

    public async Task DecreaseProductShoppingListItem(ProductShoppingListItemDto item, double count)
    {
        this.ThrowIfNotInitialized();

        var product = await _productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        if (item.Count <= count)
        {
            await RemoveProductShoppingListItem(item);
            return;
        }

        await _productService.IncreaseProductCount(product.Id, count);

        item.Count -= count;
        ShoppingListItems.AddOrUpdate(item);
        SelectedShoppingListItems.AddOrUpdate(item);
    }

    /// <summary>
    /// 
    /// <para>
    /// Warning! This method will changeSet the product count in the database!!!.
    /// </para>
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task RemoveProductShoppingListItem(ProductShoppingListItemDto product)
    {
        var productDto = await _productService.GetProductById(product.ItemId);

        if (productDto is null)
        {
            throw new InvalidOperationException($"Product with id {product.ItemId} not found.");
        }

        await _productService.IncreaseProductCount(productDto.Id, product.Count);

        ShoppingListItems.Remove(product.Id);
        SelectedShoppingListItems.Remove(product.Id);

        if (_additivesInProductDto.TryGetValue(product.Id, out var cache))
        {
            cache.Dispose();
            _additivesInProductDto.Remove(product.Id);
        }
    }

    public async ValueTask<OrderDto> GetOrder()
    {
        var order = await CreateOrGetOrder();

        order.Comment = TotalComment.Value;
        order.Products = ShoppingListItems.Values.Select(x =>
        {
            var product = Mapper.MapShoppingListItemToOrderedProductDto(x);

            product.Additives = _additivesInProductDto.TryGetValue(x.Id, out var cache)
                ? cache.Values.Select(Mapper.MapAdditiveShoppingListItemToOrderedAdditiveDto)
                : [];

            return product;

        }).ToList();

        order.TotalSum = order.Products.Sum(x => x.TotalPrice);
        order.SubtotalSum = order.Products.Sum(x => x.SubTotalPrice);
        order.Discount = Discount.Value;

        return order;
    }

    public void SetMultiSelect(bool isMultiSelect)
    {
        _isMultiSelectObservable.OnNext(isMultiSelect);
    }

    public void SetShowPrice(bool showPrice)
    {
        _showPriceObservable.OnNext(showPrice);
    }

    private async ValueTask<OrderDto> CreateOrGetOrder()
    {
        if (_order != null)
        {
            return _order;
        }

        var shiftService = await Locator.GetInitializedService<IShiftService>();

        Debug.Assert(shiftService.CurrentShift.Value != null);
        Debug.Assert(shiftService.CurrentCashierShift.Value != null);

        var cashierShift = await shiftService.CurrentCashierShift.Value.CreateDto();
        var shift = await shiftService.CurrentShift.Value.CreateDto();

        _order = new OrderDto
        {
            Status = OrderStatus.New,
            CreatedAt = DateTime.Now,
            ShiftId = shift.Id,
            CashierShiftId = cashierShift.Id,
            Id = OrderId
        };

        return _order;
    }

    public IDisposable BindFastMenu<T>(Func<FastMenuDto, IApplicationModelPresenter<FastMenuDto>> creator, out ReadOnlyObservableCollection<T> items) => throw new NotImplementedException();
}
