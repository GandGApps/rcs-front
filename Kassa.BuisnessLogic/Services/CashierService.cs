using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
internal class CashierService(IProductService productService, ICategoryService categoryService, IAdditiveService additiveService) : ICashierService
{
    private string? _totalComment;

    private readonly List<CategoryDto> _categoriesStack = [];
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

    public IReadOnlyList<CategoryDto> CategoriesStack => _categoriesStack;

    public CategoryDto? CurrentCategory
    {
        get => _currentCategory;
        set
        {
            if (_currentCategory == value)
            {
                return;
            }

            _currentCategory = value;
            PropertyChanged?.Invoke(this, new(nameof(CurrentCategory)));
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

    public SourceCache<AdditiveDto, int> AdditivesForSelectedProduct
    {
        get;
    } = new(x => x.Id);

    private CategoryDto? _currentCategory;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItemDto> categoryItems)
    {
        this.ThrowIfNotInitialized();

        var filterCondition = this.WhenPropertyChanged(cashierService => cashierService.CurrentCategory)
            .Select(category => new Func<ICategoryItemDto, bool>(item => item.CategoryId == category.Value?.Id));

        var categoryStream = categoryService.RuntimeCategories.Connect()
            .Transform(category => (ICategoryItemDto)category)
            .Filter(filterCondition);

        var productStream = productService.RuntimeProducts.Connect()
            .Transform(product => (ICategoryItemDto)product)
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

        return stream.Subscribe();
    }

    public IDisposable BindShoppingListItems<T>(Func<ProductShoppingListItemDto, T> creator, out ReadOnlyObservableCollection<T> shoppingListItems) where T : class, IReactiveToChangeSet<int, ProductShoppingListItemDto>
    {
        this.ThrowIfNotInitialized();

        var stream = ShoppingListItems.Connect()
            .TransformWithInlineUpdate(x => creator(x), (x, source) => x.Source = source)
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

    public IDisposable BindAdditivesForSelectedProduct<T>(Func<AdditiveDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<int, AdditiveDto>
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

        categoryService.Dispose();
        productService.Dispose();
        additiveService.Dispose();

        foreach (var item in _additivesInProductDto)
        {
            item.Value.Dispose();
        }

        IsDisposed = true;
    }
    public async ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await categoryService.DisposeAsync();
        await productService.DisposeAsync();
        await additiveService.DisposeAsync();

        foreach (var item in _additivesInProductDto)
        {
            item.Value.Dispose();
        }

        IsDisposed = true;
    }

    public async ValueTask Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        await productService.Initialize();
        await categoryService.Initialize();
        await additiveService.Initialize();

        IsInitialized = true;
    }

    public async Task SelectCategory(int categoryId)
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

    public async Task AddProductToShoppingList(int productId)
    {
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

    public async Task SelectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
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

    public async Task UnselectShoppingListItem(IShoppingListItemDto shoppingListItemDto)
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

    private async ValueTask UpdateAdditivesForSelectedProduct()
    {
        this.ThrowIfNotInitialized();

        AdditivesForSelectedProduct.Clear();

        foreach (var shoppingListItem in SelectedShoppingListItems.Items)
        {

            if (shoppingListItem is ProductShoppingListItemDto shoppingListItemDto)
            {
                var additives = await additiveService.GetAdditivesByProductId(shoppingListItemDto.ItemId);

                AdditivesForSelectedProduct.AddOrUpdate(additives);
            }
        }
    }

    public async Task AddAdditiveToSelectedProducts(int additiveId)
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

    public bool IsAdditiveAdded(int additiveId)
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
}
