using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface ICashierService
{
    /// <summary>
    /// <para>
    /// The products that are currently in the runtime. 
    /// </para>
    /// <para>
    /// It's a cache of the products that are in the database.
    /// Every change in the database will be reflected in this cache.
    /// </para>
    /// </summary>
    public SourceCache<Product, int> RuntimeProducts
    {
        get;
    }

    public Task UpdateProduct(Product product);
    public Task DecreaseProductCount(int productId);
    public Task DecreaseProductCount(Product product);
    public Task IncreaseProductCount(int productId);
    public Task IncreaseProductCount(Product product);

    public ValueTask<Product?> GetProductById(int productId);
}