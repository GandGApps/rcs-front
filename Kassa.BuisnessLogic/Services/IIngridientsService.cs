using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface IIngridientsService
{
    public SourceCache<IngredientDto, Guid> RuntimeIngridients
    {
        get;
    }

    public Task AddIngridient(IngredientDto ingridientDto);
    public Task DeleteIngridient(Guid id);
    public Task UpdateIngridient(IngredientDto ingridientDto);
    public ValueTask<IngredientDto?> GetIngridient(Guid id);

}
