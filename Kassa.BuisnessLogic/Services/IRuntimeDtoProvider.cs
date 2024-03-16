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
internal interface IRuntimeDtoProvider<TDto, TKey, TModel> where TModel : class, IModel where TDto : class, IDto<TModel, TDto> where TKey : notnull
{
    SourceCache<TDto, TKey> RuntimeDtos
    {
        get;
    }

    IRepository<TModel> Repository
    {
        get;
    }
}
