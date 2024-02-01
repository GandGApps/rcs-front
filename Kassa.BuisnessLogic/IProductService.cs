using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface IProductService: IInitializableService
{
    /// <summary>
    /// <para>
    /// The products that are currently in the runtime. 
    /// </para>
    /// <para>
    /// It's a cache of the products that are in the database.
    /// Every change in the database will be reflected in this cache.
    /// </para>
    /// <para>
    /// Don't use this property to update products. Use <see cref="UpdateProduct(Product)"/> instead. 
    /// Use this property only for connect.
    /// </para>
    /// </summary>
    public SourceCache<ProductDto, int> RuntimeProducts
    {
        get;
    }

    public Task UpdateProduct(ProductDto product);
    public Task RemoveProduct(ProductDto product);
    public Task DecreaseProductCount(int productId);
    public Task DecreaseProductCount(ProductDto product);
    public Task IncreaseProductCount(int productId);
    public Task IncreaseProductCount(ProductDto product);

    public ValueTask<ProductDto?> GetProductById(int productId);
}
