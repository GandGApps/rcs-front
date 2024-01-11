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
            new() { Id = 1, Name = "Холодные напитки Напитки", Price = 244, Count = 10, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 2, Name = "Мороженое Замороженные продукты", Price = 574, Count = 11, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 3, Name = "Горячий кофе Основное", Price = 429, Count = 9, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 4, Name = "Холодные напитки Замороженные продукты", Price = 788, Count = 11, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 5, Name = "Мороженое Бакалея", Price = 717, Count = 5, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 6, Name = "Макароны Напитки", Price = 124, Count = 5, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 7, Name = "Холодные напитки Напитки", Price = 341, Count = 9, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 8, Name = "Макароны Основное", Price = 592, Count = 8, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 9, Name = "Шоколад Замороженные продукты", Price = 248, Count = 9, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 10, Name = "Холодные напитки Десерты", Price = 442, Count = 8, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 11, Name = "Горячий кофе Замороженные продукты", Price = 390, Count = 11, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 12, Name = "Мороженое Основное", Price = 378, Count = 8, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 13, Name = "Горячий кофе Напитки", Price = 851, Count = 6, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 14, Name = "Чипсы Напитки", Price = 767, Count = 12, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 15, Name = "Шоколад Бакалея", Price = 642, Count = 12, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 16, Name = "Мороженое Бакалея", Price = 961, Count = 7, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 17, Name = "Холодные напитки Десерты", Price = 939, Count = 12, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 18, Name = "Горячий кофе Основное", Price = 950, Count = 10, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 19, Name = "Горячий кофе Замороженные продукты", Price = 609, Count = 8, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 20, Name = "Шоколад Замороженные продукты", Price = 609, Count = 6, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 21, Name = "Горячий кофе Снэки", Price = 887, Count = 7, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 22, Name = "Мороженое Бакалея", Price = 287, Count = 8, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 23, Name = "Холодные напитки Снэки", Price = 934, Count = 5, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 24, Name = "Горячий кофе Десерты", Price = 578, Count = 7, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 25, Name = "Макароны Бакалея", Price = 109, Count = 7, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 26, Name = "Горячий кофе Замороженные продукты", Price = 942, Count = 6, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 27, Name = "Шоколад Напитки", Price = 458, Count = 6, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 28, Name = "Шоколад Основное", Price = 723, Count = 7, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 29, Name = "Горячий кофе Замороженные продукты", Price = 820, Count = 9, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 30, Name = "Шоколад Десерты", Price = 217, Count = 6, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 31, Name = "Холодные напитки Замороженные продукты", Price = 333, Count = 10, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 32, Name = "Горячий кофе Бакалея", Price = 498, Count = 9, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 33, Name = "Мороженое Напитки", Price = 162, Count = 12, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 34, Name = "Макароны Основное", Price = 358, Count = 7, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 35, Name = "Шоколад Снэки", Price = 101, Count = 10, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 36, Name = "Холодные напитки Основное", Price = 786, Count = 5, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 37, Name = "Мороженое Напитки", Price = 755, Count = 11, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 38, Name = "Холодные напитки Десерты", Price = 411, Count = 12, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 39, Name = "Холодные напитки Снэки", Price = 134, Count = 8, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 40, Name = "Холодные напитки Бакалея", Price = 948, Count = 10, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 41, Name = "Мороженое Основное", Price = 498, Count = 5, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 42, Name = "Горячий кофе Основное", Price = 166, Count = 7, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 43, Name = "Макароны Замороженные продукты", Price = 199, Count = 12, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 44, Name = "Шоколад Снэки", Price = 686, Count = 10, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 45, Name = "Мороженое Десерты", Price = 610, Count = 12, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 46, Name = "Мороженое Напитки", Price = 945, Count = 8, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 47, Name = "Шоколад Замороженные продукты", Price = 982, Count = 12, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 48, Name = "Чипсы Замороженные продукты", Price = 636, Count = 9, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 49, Name = "Холодные напитки Снэки", Price = 154, Count = 12, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 50, Name = "Чипсы Замороженные продукты", Price = 116, Count = 8, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 51, Name = "Макароны Снэки", Price = 177, Count = 8, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 52, Name = "Шоколад Снэки", Price = 235, Count = 12, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 53, Name = "Холодные напитки Замороженные продукты", Price = 821, Count = 12, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 54, Name = "Чипсы Десерты", Price = 430, Count = 5, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 55, Name = "Горячий кофе Десерты", Price = 827, Count = 12, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 56, Name = "Шоколад Основное", Price = 496, Count = 6, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 57, Name = "Шоколад Бакалея", Price = 391, Count = 10, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 58, Name = "Чипсы Основное", Price = 914, Count = 11, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 59, Name = "Макароны Напитки", Price = 859, Count = 5, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 60, Name = "Горячий кофе Основное", Price = 543, Count = 5, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 61, Name = "Горячий кофе Снэки", Price = 317, Count = 5, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 62, Name = "Горячий кофе Снэки", Price = 384, Count = 9, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 63, Name = "Шоколад Снэки", Price = 517, Count = 9, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 64, Name = "Шоколад Бакалея", Price = 950, Count = 5, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 65, Name = "Холодные напитки Бакалея", Price = 956, Count = 10, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 66, Name = "Мороженое Снэки", Price = 653, Count = 5, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 67, Name = "Холодные напитки Десерты", Price = 339, Count = 6, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 68, Name = "Чипсы Основное", Price = 154, Count = 12, Measure = "шт", Category = "Основное", CurrencySymbol = "₽" },
            new() { Id = 69, Name = "Мороженое Бакалея", Price = 410, Count = 8, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 70, Name = "Шоколад Десерты", Price = 334, Count = 7, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 71, Name = "Шоколад Десерты", Price = 430, Count = 12, Measure = "шт", Category = "Десерты", CurrencySymbol = "₽" },
            new() { Id = 72, Name = "Макароны Замороженные продукты", Price = 651, Count = 10, Measure = "шт", Category = "Замороженные продукты", CurrencySymbol = "₽" },
            new() { Id = 73, Name = "Макароны Снэки", Price = 287, Count = 7, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 74, Name = "Холодные напитки Напитки", Price = 583, Count = 8, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" },
            new() { Id = 75, Name = "Холодные напитки Снэки", Price = 787, Count = 11, Measure = "шт", Category = "Снэки", CurrencySymbol = "₽" },
            new() { Id = 76, Name = "Мороженое Бакалея", Price = 440, Count = 12, Measure = "шт", Category = "Бакалея", CurrencySymbol = "₽" },
            new() { Id = 77, Name = "Шоколад Напитки", Price = 592, Count = 12, Measure = "шт", Category = "Напитки", CurrencySymbol = "₽" }
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

    public ValueTask<Product?> GetProductById(int productId)
    {
        var productOptional = RuntimeProducts.Lookup(productId);

        if (!productOptional.HasValue)
        {
            return ValueTask.FromResult<Product?>(null);
        }

        return new(productOptional.Value);
    }

    public Task UpdateProduct(Product product)
    {
        RuntimeProducts.AddOrUpdate(product);

        return Task.CompletedTask;
    }
}
