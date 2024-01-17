using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
internal class CashierService(IProductService productService, ICategoryService categoryService) : ICashierService
{
    public SourceCache<Product, int> RuntimeProducts
    {
        get;
    } = new(x => x.Id);
}
