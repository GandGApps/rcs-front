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
    private readonly List<CategoryDto> _categoriesStack = [];

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

        if (shoppingListItemDto is ProductShoppingListItemDto shoppingListItem)
        {
            ShoppingListItems.AddOrUpdate(shoppingListItem with
            {
                IsSelected = true
            });
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

        await UpdateAdditivesForSelectedProduct();
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

    public async Task AddAdditiveToProduct(int additiveId)
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

                var updatedAdditive = additive with
                {
                };

                product.Additives.Add(updatedAdditive);
                await additiveService.UpdateAdditive(updatedAdditive);
            }
        }

    }

    public bool IsAdditiveAdded(int additiveId)
    {
        this.ThrowIfNotInitialized();

        return ShoppingListItems.Items.Any(x => x.Additives.Any(x => x.Id == additiveId));
    }
}
