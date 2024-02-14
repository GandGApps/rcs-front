using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Dto;
internal interface IDto<TModel, TDto> where TModel : class where TDto : class
{
    [return: NotNullIfNotNull(nameof(model))]
    public static abstract TDto? FromModel(TModel? model);
    [return: NotNullIfNotNull(nameof(dto))]
    public static abstract TModel? ToModel(TDto? dto);

    public Guid Id
    {
        get;
    }
}
