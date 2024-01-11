using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface ICashierService
{
    SourceCache<Product, int> RuntimeProducts
    {
        get;
    }
}