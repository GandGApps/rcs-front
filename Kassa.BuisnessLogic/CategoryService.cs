using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public sealed class CategoryService(IRepository<Category> repository) : ICategoryService
{
    public SourceCache<CategoryDto, int> RuntimeCategories
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

            RuntimeCategories.AddOrUpdate(categories.Select(x => ToCategoryDto(x)));

            IsInitialized = true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to initialize repository", e);
        }
    }

    public async Task AddCategory(CategoryDto categoryDto)
    {
        this.ThrowIfNotInitialized();

        var category = await GetCategoryOrThrow(categoryDto, repository);

        await repository.Add(category);

        await UpdateCategory(categoryDto);
    }

    public async Task RemoveCategory(CategoryDto categoryDto)
    {
        this.ThrowIfNotInitialized();

        var category = await GetCategoryOrThrow(categoryDto, repository);

        await repository.Delete(category);

        RuntimeCategories.Remove(categoryDto);
    }

    public async ValueTask<CategoryDto?> GetCategoryById(int id)
    {
        this.ThrowIfNotInitialized();

        var category = await repository.Get(id);

        return ToCategoryDto(category);
    }

    public async Task UpdateCategory(CategoryDto categoryDto)
    {
        this.ThrowIfNotInitialized();

        var category = await GetCategoryOrThrow(categoryDto, repository);

        await repository.Update(category);

        UpdateRuntimeCategories(categoryDto);
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

    private void UpdateRuntimeCategories(CategoryDto category)
    {
        RuntimeCategories.AddOrUpdate(category);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(category))]
    private static CategoryDto? ToCategoryDto(Category? category) => CategoryDto.FromCategory(category);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task<Category> GetCategoryOrThrow(int id, IRepository<Category> repository)
    {
        var category = await repository.Get(id);

        if (category == null)
        {
            throw new InvalidOperationException($"Category with id {id} not found");
        }

        return category;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task<Category> GetCategoryOrThrow(CategoryDto categorytDto, IRepository<Category> repository) => GetCategoryOrThrow(categorytDto.Id, repository);

}
