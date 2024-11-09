using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
public sealed class CategoryService(IRepository<Category> repository) : ICategoryService
{
    public IApplicationModelManager<CategoryDto> RuntimeCategories
    {
        get;
    } = new HostModelManager<CategoryDto>();

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

            RuntimeCategories.AddOrUpdate(categories.Select(x => Mapper.MapCategoryToDto(x)));

            IsInitialized = true;
        }
        catch (Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException("Failed to initialize repository", e);
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

        RuntimeCategories.Remove(categoryDto.Id);
    }

    public async ValueTask<CategoryDto?> GetCategoryById(Guid id)
    {
        this.ThrowIfNotInitialized();

        var category = await repository.Get(id);

        return Mapper.MapCategoryToDto(category);
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
    private static async Task<Category> GetCategoryOrThrow(Guid id, IRepository<Category> repository)
    {
        var category = await repository.Get(id);

        return category ?? ThrowHelper.ThrowInvalidOperationException<Category>($"Category with id {id} not found");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task<Category> GetCategoryOrThrow(CategoryDto categorytDto, IRepository<Category> repository) => GetCategoryOrThrow(categorytDto.Id, repository);

}
