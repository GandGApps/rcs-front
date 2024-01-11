using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
internal class CashierService : ICashierService
{
    public SourceCache<Product, int> RuntimeProducts
    {
        get;
    }

    public CashierService()
    {
        RuntimeProducts = new(x => x.Id);


        RuntimeProducts.AddOrUpdate([
            new() { Id = 1, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт" },
            new() { Id = 2, Name = "", Price = 1299, Count = 1, Measure = "шт" },
            new() { Id = 3, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт" },
            new() { Id = 4, Name = "Холодные напитки “Криспи Гриль”", Price = 443, Count = 1, Measure = "шт" },
            new() { Id = 5, Name = "Холодные напитки “Криспи Гриль”", Price = 312, Count = 1, Measure = "шт" },
            new() { Id = 6, Name = "Холодные напитки “Криспи Гриль”", Price = 33, Count = 1, Measure = "шт" },
            new() { Id = 7, Name = "Холодные напитки “Криспи Гриль”", Price = 123, Count = 1, Measure = "шт" },
            new() { Id = 8, Name = "Холодные напитки “Криспи Гриль”", Price = 732, Count = 1, Measure = "шт" },
            new() { Id = 9, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт" },
            new() { Id = 10, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт" },
            new() { Id = 11, Name = "Холодные напитки “Криспи Гриль”", Price = 1231, Count = 1, Measure = "шт" },
        ]);
    }

    public Task DecreaseProductCount(Product product)
    {
        RuntimeProducts.AddOrUpdate(product with
        {
            Count = product.Count - 1
        });

        return Task.CompletedTask;
    }

    public Task IncreaseProductCount(Product product)
    {
        RuntimeProducts.AddOrUpdate(product with
        {
            Count = product.Count + 1
        });

        return Task.CompletedTask;
    }
}
