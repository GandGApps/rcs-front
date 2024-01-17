using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public sealed class CategoryService(IRepository<Category> repository) : ICategoryService
{
    public SourceCache<Category, int> RuntimeCategories
    {
        get;
    } = new(x => x.Id);

    public bool IsInitialized
    {
        get; private set;
    }

    public bool IsDisposed
    {
        get; private set;
    }

    public async ValueTask Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        try
        {
            var categories = await repository.GetAll();

            RuntimeCategories.AddOrUpdate(categories);

            IsInitialized = true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to initialize repository", e);
        }
    }

    public async Task AddCategory(Category category)
    {
        this.ThrowIfNotInitialized();

        await repository.Add(category);

        await UpdateCategory(category);
    }

    public async Task RemoveCategory(Category category)
    {
        this.ThrowIfNotInitialized();

        await repository.Delete(category);

        RuntimeCategories.Remove(category);
    }

    public async ValueTask<Category?> GetCategoryById(int id)
    {
        this.ThrowIfNotInitialized();

        return await repository.Get(id);
    }

    public async Task UpdateCategory(Category category)
    {
        this.ThrowIfNotInitialized();

        await repository.Update(category);

        UpdateRuntimeCategories(category);
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        RuntimeCategories.Dispose();

        IsDisposed = true;
    }

    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    private void UpdateRuntimeCategories(Category category)
    {
        RuntimeCategories.AddOrUpdate(category);
    }

}
