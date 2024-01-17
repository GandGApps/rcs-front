using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
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

    public IReadOnlyList<Category> CategoriesStack => _categoriesStack;

    public Category? CurrentCategory
    {
        get; private set;
    }

    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItem> categoryItems)
    {
        this.ThrowIfNotInitialized();

        var categoryStream = categoryService.RuntimeCategories.Connect()
            .Filter(category => category.CategoryId == CurrentCategory?.Id)
            .Transform(product => (ICategoryItem)product);

        var productStream = productService.RuntimeProducts.Connect()
            .Filter(product => product.CategoryId == CurrentCategory?.Id)
            .Transform(product => (ICategoryItem)product);

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
    }
    public ValueTask<bool> SelectPreviosCategory()
    {
        this.ThrowIfNotInitialized();

        if (_categoriesStack.Count == 0)
        {
            return new(false);
        }

        _categoriesStack.RemoveAt(_categoriesStack.Count - 1);

        return new(true);
    }
}
