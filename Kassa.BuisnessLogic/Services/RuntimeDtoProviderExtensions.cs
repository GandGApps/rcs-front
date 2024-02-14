using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
internal static class RuntimeDtoProviderExtensions
{
    public static async Task<TDto> GetDtoByIdOrThrow<TDto, TKey, TModel>(this IRuntimeDtoProvider<TDto, TKey, TModel> provider, Guid id)
        where TKey : notnull
        where TModel : class, IModel
        where TDto : class, IDto<TModel, TDto>
    {
        var model = await provider.Repository.Get(id);

        if (model == null)
        {
            throw new InvalidOperationException($"Product with id {id} not found");
        }

        var dto = TDto.FromModel(model);

        return dto;
    }

    public static async Task<TDto?> GetDtoAndUpdateRuntime<TDto, TKey, TModel>(this IRuntimeDtoProvider<TDto, TKey, TModel> provider, Guid id)
        where TKey : notnull
        where TModel : class, IModel
        where TDto : class, IDto<TModel, TDto>
    {
        var model = await provider.Repository.Get(id);
        var dto = TDto.FromModel(model);

        if (dto != null)
        {
            provider.RuntimeDtos.AddOrUpdate(dto);
        }

        return dto;
    }

    public static async Task UpdateDto<TDto, TKey, TModel>(this IRuntimeDtoProvider<TDto, TKey, TModel> provider, TDto dto)
        where TKey : notnull
        where TModel : class, IModel
        where TDto : class, IDto<TModel, TDto>
    {
        var dtoFounded = await provider.GetDtoByIdOrThrow(dto.Id);
        var model = TDto.ToModel(dto);

        await provider.Repository.Update(model);

        provider.RuntimeDtos.AddOrUpdate(dto);
    }


    public static async Task RemoveDto<TDto, TKey, TModel>(this IRuntimeDtoProvider<TDto, TKey, TModel> provider, TDto dto)
        where TKey : notnull
        where TModel : class, IModel
        where TDto : class, IDto<TModel, TDto>
    {
        dto = await provider.GetDtoByIdOrThrow(dto.Id);
        var model = TDto.ToModel(dto);

        await provider.Repository.Delete(model);

        provider.RuntimeDtos.Remove(dto);
    }
}
