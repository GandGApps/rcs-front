using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic.Services;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;

/// <inheritdoc cref="IStorageScope"/>
internal sealed class StorageScope(IngridientsService ingridientsService) : IStorageScope, IEnableLogger
{
    // How it's works:
    //     1) Original ingredient count is stored in the <see cref="IngridientsService.RuntimeIngridients"/>
    //     2) In that dictionary saved only the difference between the original ingredient count and the spent ingredient count
    //     3) When the scope is disposed or submitted, the difference is added to the original ingredient count

    private readonly Dictionary<Guid, double> _ingridients = new(ingridientsService.RuntimeIngridients.Count);

    private bool _isDisposedOrSubmited;

    public void Dispose()
    {
        if (CheckIsSubmitted())
        {
            return;
        }

        Submit();
    }

    public Task<bool> HasEnoughIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count)
    {
        if (ThrowOrLogIfDisposed())
        {
            return Task.FromResult(false);
        }

        foreach (var ingredientUsage in ingredientUsages)
        {
            if (TryGetIngredientCount(ingredientUsage.IngredientId, out var currentCount))
            {
                return Task.FromResult(false);
            }

            if (currentCount < ingredientUsage.Count * count)
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }

    public Task ReturnIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count)
    {
        if (ThrowOrLogIfDisposed())
        {
            return Task.CompletedTask;
        }

        foreach (var ingredientUsage in ingredientUsages)
        {

            if (TryGetIngredientCount(ingredientUsage.IngredientId, out var currentCount))
            {
                return Task.CompletedTask;
            }

            _ingridients[ingredientUsage.IngredientId] = currentCount + ingredientUsage.Count * count;
        }

        return Task.CompletedTask;
    }

    public Task SpendIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count)
    {
        if (ThrowOrLogIfDisposed())
        {
            return Task.CompletedTask;
        }

        foreach (var ingredientUsage in ingredientUsages)
        {
            if (TryGetIngredientCount(ingredientUsage.IngredientId, out var currentCount))
            {
                return Task.CompletedTask;
            }

            _ingridients[ingredientUsage.IngredientId] = currentCount - ingredientUsage.Count * count;
        }

        return Task.CompletedTask;
    }

    public void Submit()
    {
        if (CheckIsSubmitted())
        {
            return;
        }

        foreach (var (ingredientId, count) in _ingridients)
        {
            if (ingridientsService.RuntimeIngridients.TryGetValue(ingredientId, out var ingredientDto))
            {
                ingredientDto.Count += count;
                ingridientsService.RuntimeIngridients.AddOrUpdate(ingredientDto);
            }
            else
            {

                this.Log().Error("Ingredient with id {0} not found", ingredientId);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ThrowOrLogIfDisposed()
    {
        if (_isDisposedOrSubmited)
        {
#if DEBUG
            ThrowHelper.ThrowInvalidOperationException("The scope is disposed or submitted");
#elif RELEASE
            this.Log().Error("The scope is disposed or submitted");
            return false;
#endif
        }

        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetIngredientCount(IngredientDto ingredientDto, out double count)
    {
        return TryGetIngredientCount(ingredientDto.Id, out count);
    }

    private bool TryGetIngredientCount(Guid ingredientId, out double count)
    {
        // Check if the ingredient exists in the runtime ingredients
        var ingridient = ingridientsService.RuntimeIngridients.TryGetValue(ingredientId, out var ingredientFounded) ? ingredientFounded : null;

        if (ingridient is null)
        {
            this.Log().Error("Ingredient with id {0} not found", ingredientId);

            count = 0;
            return false;
        }

        // Check if the ingredient exists in the scope
        if (_ingridients.TryGetValue(ingredientId, out count))
        {

        }
        else
        {
            _ingridients[ingredientId] = count = 0;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CheckIsSubmitted()
    {
        if (_isDisposedOrSubmited)
        {
            this.Log().Warn("The scope is already disposed or submitted");
            return true;
        }
        _isDisposedOrSubmited = true;
        return false;
    }
}
