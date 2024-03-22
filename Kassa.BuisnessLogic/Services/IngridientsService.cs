using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class IngridientsService(IRepository<Ingredient> ingridients) : IIngridientsService
{
    public SourceCache<IngredientDto, Guid> RuntimeIngridients
    {
        get;
    } = new(x => x.Id);

    public async Task AddIngridient(IngredientDto ingridientDto)
    {
        var ingridient = Mapper.MapIngredientDtoToIngredient(ingridientDto);

        await ingridients.Add(ingridient);

        RuntimeIngridients.AddOrUpdate(ingridientDto);
    }
    public async Task DeleteIngridient(Guid id)
    {
        var ingridient = await ingridients.Get(id);

        if (ingridient is null)
        {
            throw new InvalidOperationException($"Ingridient with id {id} not found");
        }

        await ingridients.Delete(ingridient);

        RuntimeIngridients.RemoveKey(id);
    }
    public async ValueTask<IngredientDto?> GetIngridient(Guid id)
    {
        var ingridient = await ingridients.Get(id);

        if (ingridient is null)
        {
            return null;
        }

        var dto = Mapper.MapIngredientToIngredientDto(ingridient);

        RuntimeIngridients.AddOrUpdate(dto);

        return dto;
    }

    public Task UpdateIngridient(IngredientDto ingridientDto) => throw new NotImplementedException();
}
