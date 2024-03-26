using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class IngridientsService(IRepository<Ingredient> repository) : BaseInitializableService, IIngridientsService
{
    public SourceCache<IngredientDto, Guid> RuntimeIngridients
    {
        get;
    } = new(x => x.Id);

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var ingridients = await repository.GetAll();

        foreach (var ingridient in ingridients)
        {
            var ingridientDto = Mapper.MapIngredientToIngredientDto(ingridient);

            RuntimeIngridients.AddOrUpdate(ingridientDto);
        }
    }

    public async Task AddIngridient(IngredientDto ingridientDto)
    {
        var ingridient = Mapper.MapIngredientDtoToIngredient(ingridientDto);

        await repository.Add(ingridient);

        RuntimeIngridients.AddOrUpdate(ingridientDto);
    }
    public async Task DeleteIngridient(Guid id)
    {
        var ingridient = await repository.Get(id);

        if (ingridient is null)
        {
            throw new InvalidOperationException($"Ingridient with id {id} not found");
        }

        await repository.Delete(ingridient);

        RuntimeIngridients.RemoveKey(id);
    }
    public async ValueTask<IngredientDto?> GetIngridient(Guid id)
    {
        var ingridient = await repository.Get(id);

        if (ingridient is null)
        {
            return null;
        }

        var dto = Mapper.MapIngredientToIngredientDto(ingridient);

        RuntimeIngridients.AddOrUpdate(dto);

        return dto;
    }

    public Task<bool> HasEnoughIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count)
    {
        foreach (var usage in ingredientUsages)
        {
            if (RuntimeIngridients.Lookup(usage.IngredientId).HasValue)
            {
                var ingredientDto = RuntimeIngridients.Lookup(usage.IngredientId).Value;

                if (ingredientDto.Count < usage.Count * count)
                {
                    return Task.FromResult(false);
                }
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }

    public Task ReturnIngridients(IEnumerable<IngredientUsageDto> ingredientUsages, double count = 1)
    {
        foreach (var usage in ingredientUsages)
        {

            if (RuntimeIngridients.Lookup(usage.IngredientId).HasValue)
            {

                var ingredientDto = RuntimeIngridients.Lookup(usage.IngredientId).Value;

                ingredientDto.Count += usage.Count * count;

                RuntimeIngridients.AddOrUpdate(ingredientDto);
            }
            else
            {

                throw new InvalidOperationException($"Ingridient with id {usage.IngredientId} not found");
            }
        }

        return Task.CompletedTask;
    }

    public Task SpendIngridients(IEnumerable<IngredientUsageDto> ingredientUsages, double count = 1)
    {
        foreach (var usage in ingredientUsages)
        {

            if (RuntimeIngridients.Lookup(usage.IngredientId).HasValue)
            {

                var ingredientDto = RuntimeIngridients.Lookup(usage.IngredientId).Value;

                ingredientDto.Count -= usage.Count * count;

                RuntimeIngridients.AddOrUpdate(ingredientDto);
            }
            else
            {

                throw new InvalidOperationException($"Ingridient with id {usage.IngredientId} not found");
            }
        }

        return Task.CompletedTask;
    }

    public Task UpdateIngridient(IngredientDto ingridientDto)
    {
        var model = Mapper.MapIngredientDtoToIngredient(ingridientDto);

        return repository.Update(model);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            RuntimeIngridients.Dispose();
        }
    }
}
