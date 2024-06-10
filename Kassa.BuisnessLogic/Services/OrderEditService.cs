using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using ReactiveUI;

namespace Kassa.BuisnessLogic.Services;
internal sealed class OrderEditService(
    IProductService productService,
    ICategoryService categoryService,
    IAdditiveService additiveService,
    IReceiptService receiptService,
    OrderDto? order = null) : IOrderEditService
{
    private OrderDto? _order = order;

    private string? _totalComment;
    private ICategoryDto? _currentCategory;
    private int? _selectedFavorite;

    private readonly List<ICategoryDto> _categoriesStack = [];
    private readonly Dictionary<Guid, IApplicationModelManager<AdditiveShoppingListItemDto>> _additivesInProductDto = [];

    public bool IsInitialized
    {
        get;
        private set;
    }

    public bool IsDisposed
    {
        get;
        private set;
    }

    public bool IsMultiSelect
    {
        get => _isMultiSelect;
        set
        {
            if (_isMultiSelect == value)
            {
                return;
            }

            _isMultiSelect = value;
            PropertyChanged?.Invoke(this, new(nameof(IsMultiSelect)));

            if (!_isMultiSelect)
            {
                ClearSelectedShoppingListItems();
            }
        }
    }
    private bool _isMultiSelect;

    public IReadOnlyList<ICategoryDto> CategoriesStack => _categoriesStack;

    public ICategoryDto? CurrentCategory
    {
        get => _currentCategory;
        private set
        {
            if (_currentCategory == value)
            {
                return;
            }

            _currentCategory = value;
            PropertyChanged?.Invoke(this, new(nameof(CurrentCategory)));
        }
    }

    public int? SelectedFavourite
    {
        get => _selectedFavorite;
        private set
        {

            if (_selectedFavorite == value)
            {
                return;
            }

            _selectedFavorite = value;
            PropertyChanged?.Invoke(this, new(nameof(SelectedFavourite)));
        }
    }

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
    public double Discount
    {
        get;
        set;
    } = 1;

    public bool IsDelivery
    {
        get;
        set;
    }
    IObservableOnlyBehaviourSubject<ICategoryDto?> IOrderEditService.CurrentCategory
    {
        get;
    }
    IObservableOnlyBehaviourSubject<bool> IOrderEditService.IsMultiSelect
    {
        get;
    }
    IObservableOnlyBehaviourSubject<int?> IOrderEditService.SelectedFavourite
    {
        get;
    }
    IObservableOnlyBehaviourSubject<double> IOrderEditService.Discount
    {
        get;
    }
    public IObservableOnlyBehaviourSubject<string?> TotalComment
    {
        get;
    }
    public IObservableOnlyBehaviourSubject<bool> ShowPrice
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItemDto> categoryItems)
    {
        this.ThrowIfNotInitialized();

        var target = new ObservableCollection<ICategoryItemDto>();
        var ordered = new ObservableCollection<ICategoryItemDto>();
        categoryItems = new(ordered);


        IDisposable? observeCategories = null;
        IDisposable? observeproducts = null;

        var disposable = this.WhenAnyValue(x => x.CurrentCategory)
            .Subscribe(x =>
            {
                target.Clear();

                observeCategories?.Dispose();
                observeproducts?.Dispose();

                if (x is FavouriteCategoryDto favouriteCategory)
                {
                    target.AddRange(productService.RuntimeProducts.Values
                        .Cast<ICategoryItemDto>()
                        .Where(x => x.Favourites.Contains(favouriteCategory.Favourite)));

                    observeproducts = productService.RuntimeProducts
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
                                        target.Add(change.Current);
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(change.Current);
                                        break;

                                    case ModelChangeReason.Refresh:
                                        target.Remove(change.Previous ?? change.Current);
                                        target.Add(change.Current);
                                        break;
                                }
                            }
                        });

                    observeCategories = categoryService.RuntimeCategories.Connect()
                        .Filter(c => c.Favourites.Contains(favouriteCategory.Favourite))
                        .Subscribe(changeSet =>
                        {
                            foreach (var change in changeSet)
                            {

                                switch (change.Reason)
                                {

                                    case ChangeReason.Add:
                                        target.Add(change.Current);
                                        break;

                                    case ChangeReason.Remove:
                                        target.Remove(change.Current);
                                        break;

                                    case ChangeReason.Refresh:
                                        target.Remove(change.Previous.Value);
                                        target.Add(change.Current);
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

                if (x is CategoryDto category)
                {
                    target.AddRange(categoryService.RuntimeCategories.Items
                                               .Where(c => c.CategoryId == category.Id));

                    observeCategories = categoryService.RuntimeCategories.Connect()
                        .Filter(c => c.CategoryId == category.Id)
                        .Subscribe(changeSet =>
                        {
                            foreach (var change in changeSet)
                            {
                                switch (change.Reason)
                                {
                                    case ChangeReason.Add:
                                        target.Add(change.Current);
                                        break;

                                    case ChangeReason.Remove:
                                        target.Remove(change.Current);
                                        break;

                                    case ChangeReason.Refresh:
                                        target.Remove(change.Previous.Value);
                                        target.Add(change.Current);
                                        break;
                                }
                            }
                        });

                    target.AddRange(productService.RuntimeProducts.Values
                            .Cast<ICategoryItemDto>()
                            .Where(p => p.CategoryId == category.Id));

                    observeproducts = productService.RuntimeProducts
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
                                        target.Add(change.Current);
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(change.Current);
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
                    observeCategories = categoryService.RuntimeCategories.Connect()
                        .Filter(c => c.CategoryId == null)
                        .Subscribe(changeSet =>
                        {

                            foreach (var change in changeSet)
                            {

                                switch (change.Reason)
                                {

                                    case ChangeReason.Add:
                                        target.Add(change.Current);
                                        break;

                                    case ChangeReason.Remove:
                                        target.Remove(change.Current);
                                        break;

                                    case ChangeReason.Refresh:
                                        target.Remove(change.Previous.Value);
                                        target.Add(change.Current);
                                        break;
                                }
                            }

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
                        });

                    target.AddRange(productService.RuntimeProducts.Values
                        .Cast<ICategoryItemDto>()
                        .Where(p => p.CategoryId == null));

                    observeproducts = productService.RuntimeProducts
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
                                        target.Add(change.Current);
                                        break;

                                    case ModelChangeReason.Remove:
                                        target.Remove(change.Current);
                                        break;

                                    case ModelChangeReason.Refresh:
                                        target.Remove(change.Previous ?? change.Current);
                                        target.Add(change.Current);
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
                            observableCollection.AddRange(additiveService.RuntimeAdditives.Values
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

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        foreach (var item in _additivesInProductDto)
        {
            item.Value.Dispose();
        }

        IsDisposed = true;
    }
    public ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return ValueTask.CompletedTask;
        }

        foreach (var item in _additivesInProductDto)
        {
            item.Value.Dispose();
        }

        IsDisposed = true;

        return ValueTask.CompletedTask;
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

        var category = await categoryService.GetCategoryById(categoryId);

        if (category is null)
        {
            throw new InvalidOperationException($"Category with id {categoryId} not found.");
        }

        _categoriesStack.Add(category);
        CurrentCategory = category;
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
            CurrentCategory = null;
        }
        else
        {

            CurrentCategory = _categoriesStack[^1];
        }

        return new(true);
    }

    public ValueTask SelectFavourite(int favourite)
    {
        this.ThrowIfNotInitialized();

        _categoriesStack.Clear();

        CurrentCategory = new FavouriteCategoryDto(favourite);
        SelectedFavourite = favourite;


        return ValueTask.CompletedTask;
    }

    public ValueTask SelectRootCategory()
    {
        CurrentCategory = null;
        SelectedFavourite = null;
        _categoriesStack.Clear();

        return ValueTask.CompletedTask;
    }

    public async Task AddProductToShoppingList(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productService.GetProductById(productId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found.");
        }

        var receipt = await receiptService.GetReceipt(product.ReceiptId);

        if (receipt is null)
        {
            throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found.");
        }

        if (!await receiptService.HasEnoughIngridients(receipt))
        {
            throw new InvalidOperationException($"Not enough ingredients for product {product.Name}");
        }

        await productService.DecreaseProductCount(product.Id);

        product = (await productService.GetProductById(productId))!;



        var existItem = ShoppingListItems.Values.FirstOrDefault(x => x.ItemId == product.Id);

        if (existItem != null)
        {
            existItem.Count++;
            ShoppingListItems.AddOrUpdate(existItem);

            product.IsAdded = true;
            productService.RuntimeProducts.AddOrUpdate(product);

            return;
        }

        product.IsAdded = true;
        productService.RuntimeProducts.AddOrUpdate(product);

        var shoppingListItem = new ProductShoppingListItemDto(product)
        {
            Id = Guid.NewGuid()
        };

        ShoppingListItems.AddOrUpdate(shoppingListItem);
    }

    public async Task AddProductToShoppingList(OrderedProductDto orderedProduct)
    {
        this.ThrowIfNotInitialized();

        var product = await productService.GetProductById(orderedProduct.ProductId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {orderedProduct.ProductId} not found.");
        }

        var shoppingListItem = new ProductShoppingListItemDto(orderedProduct, product);

        ShoppingListItems.AddOrUpdate(shoppingListItem);

        var sourceCache = new HostModelManager<AdditiveShoppingListItemDto>();

        foreach (var orderedAdditive in orderedProduct.Additives)
        {

            var additive = await additiveService.GetAdditiveById(orderedAdditive.AdditiveId);

            if (additive is null)
            {
                throw new InvalidOperationException($"Additive with id {orderedAdditive.AdditiveId} not found.");
            }

            var additiveShoppingListItem = new AdditiveShoppingListItemDto(orderedAdditive, shoppingListItem, additive);

            sourceCache.AddOrUpdate(additiveShoppingListItem);
        }

        _additivesInProductDto.Add(product.Id, sourceCache);
    }

    public Task SelectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {
            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }

        if (!IsMultiSelect)
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
            var product = await productService.GetProductById(shoppingListItem.ItemId);
            if (product != null)
            {
                product.IsAdded = false;

                await productService.IncreaseProductCount(product.Id);
                productService.RuntimeProducts.AddOrUpdate(product);
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

        var additive = await additiveService.GetAdditiveById(additiveId);

        if (additive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveId} not found.");
        }

        foreach (var shoppingListItem in SelectedShoppingListItems.Values)
        {
            if (additive.ProductIds.Contains(shoppingListItem.ItemId) &&
                shoppingListItem is ProductShoppingListItemDto product)
            {
                await additiveService.DecreaseAddtiveCount(additive);

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
        _totalComment = totalComment;

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
                var additiveDto = await additiveService.GetAdditiveById(additive.ItemId);

                if (additiveDto is null)
                {
                    throw new InvalidOperationException($"Additive with id {additive.ItemId} not found.");
                }

                await additiveService.IncreaseAdditiveCount(additiveDto);

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

        var product = await productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        var receipt = await receiptService.GetReceipt(product.ReceiptId);

        if (receipt is null)
        {
            throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found.");
        }

        if (!await receiptService.HasEnoughIngridients(receipt))
        {
            throw new InvalidOperationException($"Not enough ingredients for product {product.Name}");
        }

        await productService.DecreaseProductCount(product.Id, count);
        item.Count += count;
        ShoppingListItems.AddOrUpdate(item);
        SelectedShoppingListItems.AddOrUpdate(item);
    }

    public async Task DecreaseProductShoppingListItem(ProductShoppingListItemDto item, double count)
    {
        this.ThrowIfNotInitialized();

        var product = await productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        if (item.Count <= count)
        {
            await RemoveProductShoppingListItem(item);
            return;
        }

        await productService.IncreaseProductCount(product.Id, count);

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
        var productDto = await productService.GetProductById(product.ItemId);

        if (productDto is null)
        {
            throw new InvalidOperationException($"Product with id {product.ItemId} not found.");
        }

        await productService.IncreaseProductCount(productDto.Id, product.Count);

        ShoppingListItems.Remove(product.Id);
        SelectedShoppingListItems.Remove(product.Id);

        if (_additivesInProductDto.TryGetValue(product.Id, out var cache))
        {
            cache.Dispose();
            _additivesInProductDto.Remove(product.Id);
        }
    }

    public ValueTask<OrderDto> GetOrder()
    {
        var order = CreateOrGetOrder();

        order.Comment = _totalComment!;
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
        order.Discount = Discount;

        return new(order);
    }

    private OrderDto CreateOrGetOrder()
    {
        _order ??= new OrderDto
        {
            Status = OrderStatus.New,
            CreatedAt = DateTime.Now,
        };

        _order.Id = OrderId;

        return _order;
    }

    public IDisposable BindFastMenu<T>(Func<FastMenuDto, IApplicationModelPresenter<FastMenuDto>> creator, out ReadOnlyObservableCollection<T> items) => throw new NotImplementedException();
    public void SetMultiSelect(bool isMultiSelect) => throw new NotImplementedException();
    public void SetShowPrice(bool showPrice) => throw new NotImplementedException();
}
