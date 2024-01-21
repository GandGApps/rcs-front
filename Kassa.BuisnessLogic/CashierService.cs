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
using DynamicData.PLinq;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
internal class CashierService(IProductService productService, ICategoryService categoryService) : ICashierService
{
    private readonly List<Category> _categoriesStack = [];

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

    public IReadOnlyList<Category> CategoriesStack => _categoriesStack;

    public Category? CurrentCategory
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

    public SourceCache<ShoppingListItemDto, Guid> ShoppingListItems
    {
        get;
    } = new(x => x.Id);

    public SourceCache<IShoppingListItemDto, Guid> SelectedShoppingListItems
    {
        get;
    } = new(x => x.Id);

    public SourceCache<Additive, int> AdditivesForSelectedProduct
    {
        get;
    } = new(x => x.Id);

    private Category? _currentCategory;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItem> categoryItems)
    {
        this.ThrowIfNotInitialized();

        var filterCondition = this.WhenPropertyChanged(cashierService => cashierService.CurrentCategory)
            .Select(category => new Func<ICategoryItem, bool>(item => item.CategoryId == category.Value?.Id));

        var categoryStream = categoryService.RuntimeCategories.Connect()
            .Transform(product => (ICategoryItem)product)
            .Filter(filterCondition);

        var productStream = productService.RuntimeProducts.Connect()
            .Transform(product => (ICategoryItem)product)
            .Filter(filterCondition);

        var stream = categoryStream.Merge(productStream)
            .Sort(SortExpressionComparer<ICategoryItem>.Ascending(x =>
            {
                if (x is Product)
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

    public IDisposable BindShoppingListItems(out ReadOnlyObservableCollection<ShoppingListItemDto> shoppingListItems)
    {
        this.ThrowIfNotInitialized();

        var stream = ShoppingListItems.Connect()
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

    public IDisposable BindAdditives(out ReadOnlyObservableCollection<Additive> additives)
    {

        this.ThrowIfNotInitialized();

        var stream = AdditivesForSelectedProduct.Connect()
                    .Bind(out additives);

        return stream.Subscribe();
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

        await productService.DecreaseProductCount(product.Id);

        product = await productService.GetProductById(product.Id);

        var shoppingListItem = new ShoppingListItemDto(product)
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

        if (shoppingListItemDto is ShoppingListItemDto shoppingListItem)
        {
            ShoppingListItems.AddOrUpdate(shoppingListItem with
            {
                IsSelected = true
            });
        }

        await UpdateAdditivesForSelectedProduct();
    }

    public async Task RemoveShoppingListItem(IShoppingListItemDto shoppingListItemDto)
    {
        this.ThrowIfNotInitialized();

        if (shoppingListItemDto is null)
        {

            throw new ArgumentNullException(nameof(shoppingListItemDto));
        }



        if (shoppingListItemDto is ShoppingListItemDto shoppingListItem)
        {
            var product = await productService.GetProductById(shoppingListItem.ItemId);
            if (product != null)
            {

            }

            SelectedShoppingListItems.Remove(shoppingListItemDto);
            ShoppingListItems.Remove(shoppingListItem with
            {
                IsSelected = false
            });
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

        if (shoppingListItemDto is ShoppingListItemDto shoppingListItem)
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

            if (shoppingListItem is ShoppingListItemDto shoppingListItemDto)
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

            if (shoppingListItem is ShoppingListItemDto shoppingListItemDto)
            {
                var additives = await productService.GetAdditivesByProductId(shoppingListItemDto.ItemId);

                AdditivesForSelectedProduct.AddOrUpdate(additives);
            }
        }
    }
}
