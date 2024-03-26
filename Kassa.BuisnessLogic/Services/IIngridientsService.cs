using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IIngridientsService: IInitializableService
{
    public SourceCache<IngredientDto, Guid> RuntimeIngridients
    {
        get;
    }

    public Task AddIngridient(IngredientDto ingridientDto);
    public Task DeleteIngridient(Guid id);
    public Task UpdateIngridient(IngredientDto ingridientDto);
    public Task<bool> HasEnoughIngredients(IEnumerable<IngredientUsageDto> ingredientUsages, double count);
    public ValueTask<IngredientDto?> GetIngridient(Guid id);
    public Task SpendIngridients(IEnumerable<IngredientUsageDto> ingredientUsages, double count = 1);
    public Task ReturnIngridients(IEnumerable<IngredientUsageDto> ingredientUsages, double count = 1);
}
