using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using ReactiveUI;

namespace Kassa.BuisnessLogic.Services;
internal class OrderEditService(IProductService productService, ICategoryService categoryService, IAdditiveService additiveService, OrderDto? order = null) : IOrderEditService
{
    private OrderDto? _order = order;

    private string? _totalComment;
    private ICategoryDto? _currentCategory;
    private int? _selectedFavorite;

    private readonly List<ICategoryDto> _categoriesStack = [];
    private readonly Dictionary<Guid, SourceCache<AdditiveShoppingListItemDto, Guid>> _additivesInProductDto = [];

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

    public SourceCache<ProductShoppingListItemDto, Guid> ShoppingListItems
    {
        get;
    } = new(x => x.Id);

    public SourceCache<IShoppingListItemDto, Guid> SelectedShoppingListItems
    {
        get;
    } = new(x => x.Id);

    public SourceCache<AdditiveDto, Guid> AdditivesForSelectedProduct
    {
        get;
    } = new(x => x.Id);
    public Guid OrderId
    {
        get; set;
    }
    public double Discount
    {
        get;
        set;
    } = 1;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItemDto> categoryItems)
    {
        this.ThrowIfNotInitialized();

        var filterCondition = this.WhenAnyValue(x => x.CurrentCategory)
            .Select(x =>
                new Func<ICategoryItemDto, bool>(item =>
                {
                    if (x is FavouriteCategoryDto favourite)
                    {
                        Debug.WriteLineIf(item.Favourites.Contains(favourite.Favourite), $"Current category is favourite {favourite.Favourite}");
                        Debug.WriteLineIf(item.Favourites.Contains(favourite.Favourite), $"item '{item.Name}' favourites:[{string.Join(',', item.Favourites)}]");
                        return item.Favourites.Contains(favourite.Favourite);
                    }
                    if (x is CategoryDto category)
                    {
                        Debug.WriteLineIf(item.CategoryId == category.Id, $"Current category is '{category.Name}' Id={category.Id}");
                        Debug.WriteLineIf(item.CategoryId == category.Id, $"item '{item.Name}' CategoryId={item.CategoryId}");
                        return item.CategoryId == category.Id;
                    }
                    Debug.WriteLineIf(item.CategoryId == null, "Current category is root");
                    Debug.WriteLineIf(item.CategoryId == null, $"item '{item.Name}' CategoryId={item.CategoryId}");
                    return item.CategoryId == null;
                }));

        var categoryStream = categoryService.RuntimeCategories.Connect()
            .Cast(category => (ICategoryItemDto)category)
            .Filter(filterCondition);

        var productStream = productService.RuntimeProducts.Connect()
            .Cast(product => (ICategoryItemDto)product)
            .Filter(filterCondition);

        var stream = categoryStream.Merge(productStream)
            .Sort(SortExpressionComparer<ICategoryItemDto>.Ascending(x =>
            {
                if (x is ProductDto)
                {
                    return 1;
                }
                else
                {

                    return 0;
                }
            }))
            .Bind(out categoryItems);

        return stream.Subscribe(_ => { }, exc => Debug.WriteLine(exc));
    }

    public IDisposable BindShoppingListItems<T>(Func<ProductShoppingListItemDto, T> creator, out ReadOnlyObservableCollection<T> shoppingListItems) where T : class, IReactiveToChangeSet<Guid, ProductShoppingListItemDto>
    {
        this.ThrowIfNotInitialized();

        var stream = ShoppingListItems.Connect()
            .TransformWithInlineUpdate(x => creator(x), (x, source) => x.Source = source)
            .Do(x =>
            {
                if (ShoppingListItems.Count == 1)
                {
                    IsMultiSelect = false;
                    var product = ShoppingListItems.Items.First();

                    if(product.IsSelected) return;

                    product = product with
                    {
                        IsSelected = true
                    };

                    ShoppingListItems.AddOrUpdate(product);
                }
            })
            .Bind(out shoppingListItems);


        return stream.Subscribe();
    }

    public IDisposable BindSelectedShoppingListItems(out ReadOnlyObservableCollection<IShoppingListItemDto> shoppingListItems)
    {

        this.ThrowIfNotInitialized();

        var stream = SelectedShoppingListItems.Connect()
            .Bind(out shoppingListItems);

        return stream.Subscribe();
    }

    public IDisposable BindAdditivesForSelectedProduct<T>(Func<AdditiveDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<Guid, AdditiveDto>
    {
        this.ThrowIfNotInitialized();

        var subscribe = ShoppingListItems.Connect()
            .Filter(x => x.IsSelected)
            .TransformAsync(async x =>
            {
                return await additiveService.GetAdditivesByProductId(x.ItemId);
            })
            .TransformMany(x => x, x => x.Id)
            .TransformWithInlineUpdate(x => creator(x), (x, source) => x.Source = source)
            .Bind(out additives)
            .Subscribe();


        return subscribe;
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

        if (product.Count == 0)
        {
            return;
        }

        await productService.DecreaseProductCount(product.Id);

        product = (await productService.GetProductById(productId))!;

        var existItem = ShoppingListItems.Items.FirstOrDefault(x => x.ItemId == product.Id);

        if (existItem != null)
        {
            ShoppingListItems.AddOrUpdate(existItem with
            {
                Count = existItem.Count + 1
            });

            productService.RuntimeProducts.AddOrUpdate(product with
            {
                IsAdded = true
            });

            return;
        }

        productService.RuntimeProducts.AddOrUpdate(product with
        {
            IsAdded = true
        });

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

        var sourceCache = new SourceCache<AdditiveShoppingListItemDto, Guid>(x => x.Id);

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
            var selected = product with
            {
                IsSelected = true
            };

            ShoppingListItems.AddOrUpdate(selected);
            SelectedShoppingListItems.AddOrUpdate(selected);
        }

        if (shoppingListItemDto is AdditiveShoppingListItemDto additive)
        {
            if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
            {
                var selected = additive with
                {
                    IsSelected = true
                };
                sourceCache.AddOrUpdate(selected);
                SelectedShoppingListItems.AddOrUpdate(selected);
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
                await productService.IncreaseProductCount(product.Id);
                productService.RuntimeProducts.AddOrUpdate(product with
                {
                    IsAdded = false
                });
            }

            SelectedShoppingListItems.Remove(shoppingListItemDto);
        }
    }

    public Task UnselectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {
            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }

        SelectedShoppingListItems.Remove(shoppingListItemDto);

        if (shoppingListItemDto is ProductShoppingListItemDto shoppingListItem)
        {
            ShoppingListItems.AddOrUpdate(shoppingListItem with
            {
                IsSelected = false
            });
        }

        if (shoppingListItemDto is AdditiveShoppingListItemDto additive)
        {

            if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
            {
                sourceCache.AddOrUpdate(additive with
                {
                    IsSelected = false
                });
            }
        }

        return Task.CompletedTask;
    }

    private void ClearSelectedShoppingListItems()
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {

            if (shoppingListItem is ProductShoppingListItemDto shoppingListItemDto)
            {

                ShoppingListItems.AddOrUpdate(shoppingListItemDto with
                {
                    IsSelected = false
                });
            }

            if (shoppingListItem is AdditiveShoppingListItemDto additive)
            {

                if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
                {

                    sourceCache.AddOrUpdate(additive with
                    {
                        IsSelected = false
                    });
                }
            }
        }
        SelectedShoppingListItems.Clear();
        AdditivesForSelectedProduct.Clear();
    }

    public async Task AddAdditiveToSelectedProducts(Guid additiveId)
    {

        this.ThrowIfNotInitialized();

        var additive = await additiveService.GetAdditiveById(additiveId);

        if (additive is null)
        {
            throw new InvalidOperationException($"Additive with id {additiveId} not found.");
        }

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {
            if (additive.ProductIds.Contains(shoppingListItem.ItemId) &&
                additive.Count > 0 &&
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

        return SelectedShoppingListItems.Items.Select(x => x.Id)
            .Where(_additivesInProductDto.ContainsKey)
            .Select(id => _additivesInProductDto[id].Items)
            .Any(x => x.Any(additive => additive.ItemId == additiveId));
    }

    public IDisposable BindAdditivesForProductShoppingListItem<T>(ProductShoppingListItemDto item, Func<AdditiveShoppingListItemDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<Guid, AdditiveShoppingListItemDto>
    {
        this.ThrowIfNotInitialized();

        if (_additivesInProductDto.TryGetValue(item.Id, out var sourceCache))
        {
            return sourceCache.Connect()
                .TransformAndBind(creator)
                .Bind(out additives)
                .Subscribe();
        }

        sourceCache = new SourceCache<AdditiveShoppingListItemDto, Guid>(x => x.Id);

        _additivesInProductDto.Add(item.Id, sourceCache);


        return sourceCache.Connect()
                .TransformAndBind(creator)
                .Bind(out additives)
                .Subscribe();
    }

    public Task WriteCommentToSelectedItems(string? comment)
    {
        this.ThrowIfNotInitialized();


        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {

            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                product = product with
                {
                    AdditiveInfo = comment
                };
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

    public async Task IncreaseSelectedProductShoppingListItem()
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {
            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                await IncreaseProductShoppingListItem(product);
            }
        }
    }

    public async Task DecreaseSelectedProductShoppingListItem()
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {

            if (shoppingListItem is ProductShoppingListItemDto product)
            {
                await DecreaseProductShoppingListItem(product);
            }
        }
    }

    public async Task RemoveSelectedProductShoppingListItem()
    {
        this.ThrowIfNotInitialized();

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
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

                await additiveService.UpdateAdditive(additiveDto with
                {
                    Count = additiveDto.Count + additive.Count
                });

                if (_additivesInProductDto.TryGetValue(additive.ContainingProduct.Id, out var sourceCache))
                {
                    sourceCache.Remove(additive.Id);
                }
                SelectedShoppingListItems.Remove(additive.Id);
            }
        }
    }

    private async Task IncreaseProductShoppingListItem(ProductShoppingListItemDto item)
    {
        this.ThrowIfNotInitialized();

        var product = await productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        if (product.Count <= 0)
        {
            return;
        }

        await productService.DecreaseProductCount(product.Id);
        var increased = item with
        {
            Count = item.Count + 1
        };
        ShoppingListItems.AddOrUpdate(increased);
        SelectedShoppingListItems.AddOrUpdate(increased);
    }

    private async Task DecreaseProductShoppingListItem(ProductShoppingListItemDto item)
    {
        this.ThrowIfNotInitialized();

        var product = await productService.GetProductById(item.ItemId);

        if (product is null)
        {
            throw new InvalidOperationException($"Product with id {item.ItemId} not found.");
        }

        if (item.Count <= 1)
        {
            await RemoveProductShoppingListItem(item);
            return;
        }

        await productService.IncreaseProductCount(product.Id);

        var decreased = item with
        {
            Count = item.Count - 1
        };
        ShoppingListItems.AddOrUpdate(decreased);
        SelectedShoppingListItems.AddOrUpdate(decreased);
    }

    /// <summary>
    /// 
    /// <para>
    /// Warning! This method will change the product count in the database!!!.
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

        await productService.UpdateProduct(productDto with
        {
            Count = productDto.Count + product.Count
        });

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
        order.Products = ShoppingListItems.Items.Select(x =>
        {
            var product = Mapper.MapShoppingListItemToOrderedProductDto(x);

            product.Additives = _additivesInProductDto.TryGetValue(x.Id, out var cache)
                ? cache.Items.Select(Mapper.MapAdditiveShoppingListItemToOrderedAdditiveDto)
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

}
