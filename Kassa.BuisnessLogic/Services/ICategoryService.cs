using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
public interface ICategoryService : IInitializableService
{
    /// <summary>
    /// <para>
    /// The categories that are currently in the runtime. 
    /// </para>
    /// <para>
    /// It's a cache of the categories that are in the database.
    /// Every change in the database will be reflected in this cache.
    /// </para>
    /// <para>
    /// Don't use this property to update categories. Use <see cref="UpdateCategory(CategoryDto)"/> instead. 
    /// Use this property only for connect.
    /// </para>
    /// </summary>
    public SourceCache<CategoryDto, int> RuntimeCategories
    {
        get;
    }

    public Task UpdateCategory(CategoryDto category);
    public Task AddCategory(CategoryDto category);
    public Task RemoveCategory(CategoryDto category);

    public ValueTask<CategoryDto?> GetCategoryById(int categoryId);

}
