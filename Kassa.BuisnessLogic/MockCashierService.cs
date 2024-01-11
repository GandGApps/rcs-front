using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
internal class MockCashierService : ICashierService
{
    public SourceCache<Product, int> RuntimeProducts
    {
        get;
    }

    public MockCashierService()
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
        return DecreaseProductCount(product.Id);
    }

    public Task IncreaseProductCount(Product product)
    {
        return IncreaseProductCount(product.Id);
    }

    public Task DecreaseProductCount(int productId)
    {
        var productOptional = RuntimeProducts.Lookup(productId);

        if (!productOptional.HasValue)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var product = productOptional.Value;

        RuntimeProducts.Edit(updater =>
        {
            updater.AddOrUpdate(product with
            {

                Count = product.Count - 1
            });
        });

        return Task.CompletedTask;
    }
    public Task IncreaseProductCount(int productId)
    {
        var productOptional = RuntimeProducts.Lookup(productId);

        if (!productOptional.HasValue)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var product = productOptional.Value;

        RuntimeProducts.Edit(updater =>
        {
            updater.AddOrUpdate(product with
            {
                Count = product.Count + 1
            });
        });

        return Task.CompletedTask;
    }
}
