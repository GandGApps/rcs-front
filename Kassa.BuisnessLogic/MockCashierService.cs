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
            new() { Id = 1, Name = "Оливки", Price = 342, Count = 5, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 2, Name = "Конфеты", Price = 630, Count = 12, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 3, Name = "Вино", Price = 300, Count = 10, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 4, Name = "Торты", Price = 679, Count = 8, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 5, Name = "Вино", Price = 764, Count = 10, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 6, Name = "Сыры", Price = 333, Count = 12, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 7, Name = "Оливки", Price = 935, Count = 12, Measure = "шт", Categories = ["избранное1"], CurrencySymbol = "₽" },
            new() { Id = 8, Name = "Гурме йогурты", Price = 632, Count = 6, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 9, Name = "Макароны", Price = 439, Count = 10, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 10, Name = "Органические соки", Price = 800, Count = 10, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 11, Name = "Крафтовые напитки", Price = 670, Count = 12, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 12, Name = "Макароны", Price = 548, Count = 12, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 13, Name = "Крафтовые напитки", Price = 687, Count = 10, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 14, Name = "Трюфели", Price = 792, Count = 8, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 15, Name = "Гурме йогурты", Price = 447, Count = 9, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 16, Name = "Органические соки", Price = 991, Count = 12, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 17, Name = "Макароны", Price = 660, Count = 7, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 18, Name = "Импортный кофе", Price = 506, Count = 6, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 19, Name = "Макароны", Price = 846, Count = 11, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 20, Name = "Макароны", Price = 763, Count = 8, Measure = "шт", Categories = ["избранное2"], CurrencySymbol = "₽" },
            new() { Id = 21, Name = "Эко снэки", Price = 783, Count = 6, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 22, Name = "Фермерские продукты", Price = 699, Count = 12, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 23, Name = "Безглютеновые товары", Price = 506, Count = 6, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 24, Name = "Фермерские продукты", Price = 126, Count = 12, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 25, Name = "Веган продукты", Price = 666, Count = 6, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 26, Name = "Натуральные сиропы", Price = 687, Count = 8, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 27, Name = "Эко снэки", Price = 134, Count = 9, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 28, Name = "Био напитки", Price = 905, Count = 10, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 29, Name = "Натуральные сиропы", Price = 987, Count = 11, Measure = "шт", Categories = ["избранное3"], CurrencySymbol = "₽" },
            new() { Id = 30, Name = "Фри", Price = 169, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 31, Name = "Хот-дог", Price = 637, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 32, Name = "Кола", Price = 324, Count = 5, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 33, Name = "Пицца", Price = 489, Count = 8, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 34, Name = "Наггетсы", Price = 146, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 35, Name = "Сок", Price = 771, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 36, Name = "Чай", Price = 557, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 37, Name = "Кофе", Price = 860, Count = 6, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 38, Name = "Хот-дог", Price = 256, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 39, Name = "Смузи", Price = 380, Count = 10, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 40, Name = "Наггетсы", Price = 956, Count = 11, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 41, Name = "Смузи", Price = 700, Count = 10, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 42, Name = "Роллы", Price = 452, Count = 12, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 43, Name = "Кола", Price = 235, Count = 11, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 44, Name = "Чай", Price = 453, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 45, Name = "Наггетсы", Price = 690, Count = 7, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 46, Name = "Бургер", Price = 261, Count = 9, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 47, Name = "Роллы", Price = 431, Count = 5, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 48, Name = "Бургер", Price = 880, Count = 8, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 49, Name = "Лимонад", Price = 644, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 50, Name = "Вода", Price = 677, Count = 8, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 51, Name = "Бургер", Price = 897, Count = 8, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 52, Name = "Кола", Price = 158, Count = 10, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 53, Name = "Фри", Price = 433, Count = 8, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 54, Name = "Вода", Price = 997, Count = 5, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 55, Name = "Кофе", Price = 766, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 56, Name = "Роллы", Price = 546, Count = 9, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 57, Name = "Вода", Price = 130, Count = 9, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 58, Name = "Хот-дог", Price = 986, Count = 12, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 59, Name = "Кофе", Price = 605, Count = 11, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 60, Name = "Смузи", Price = 152, Count = 6, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 61, Name = "Чай", Price = 995, Count = 8, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 62, Name = "Чай", Price = 632, Count = 9, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 63, Name = "Чай", Price = 203, Count = 5, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 64, Name = "Пицца", Price = 742, Count = 8, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 65, Name = "Сок", Price = 597, Count = 8, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 66, Name = "Смузи", Price = 979, Count = 11, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 67, Name = "Сок", Price = 104, Count = 6, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 68, Name = "Бургер", Price = 565, Count = 7, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 69, Name = "Смузи", Price = 217, Count = 8, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 70, Name = "Лимонад", Price = 910, Count = 10, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 71, Name = "Бургер", Price = 701, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 72, Name = "Чай", Price = 277, Count = 8, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 73, Name = "Вода", Price = 238, Count = 7, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 74, Name = "Вода", Price = 319, Count = 6, Measure = "шт", Categories = ["напитки"], CurrencySymbol = "₽" },
            new() { Id = 75, Name = "Бургер", Price = 540, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 76, Name = "Фри", Price = 179, Count = 6, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" },
            new() { Id = 77, Name = "Бургер", Price = 130, Count = 7, Measure = "шт", Categories = ["фаст фуд"], CurrencySymbol = "₽" }
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
