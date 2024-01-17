using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface ICategoryService: IInitializableService
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
    /// Don't use this property to update categories. Use <see cref="UpdateCategory(Category)"/> instead. 
    /// Use this property only for connect.
    /// </para>
    /// </summary>
    public SourceCache<Category, int> RuntimeCategories
    {
        get;
    }

    public Task UpdateCategory(Category category);
    public Task AddCategory(Category category);
    public Task RemoveCategory(Category category);

    public ValueTask<Category?> GetCategoryById(int categoryId);

}
