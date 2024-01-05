using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class CashierVm : PageViewModel
{
    public CashierVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
        ShoppingList = new();

        // Mock data

        _fastAddictives = new([
            new() { Name = "Сыр чеддер", Count = 15, Price = 18, СurrencySymbol = "₽", Measure = "гр", IsAvailable = true },
            new() { Name = "Сыр моцарелла", Count = 15, Price = 28, СurrencySymbol = "₽", Measure = "гр", IsAvailable = true },
            new() { Name = "Сыр пармезан", Count = 15, Price = 38, СurrencySymbol = "₽", Measure = "гр", IsAvailable = true },
            new() { Name = "Сыр пармезан", Count = 15, Price = 8, СurrencySymbol = "₽", Measure = "гр", IsAvailable = true },
            new() { Name = "Сыр пармезан", Count = 15, Price = 3, СurrencySymbol = "₽", Measure = "гр", IsAvailable = true },
            new() { Name = "Сыр пармезан", Count = 15, Price = 1, СurrencySymbol = "₽", Measure = "гр", IsAvailable = false },
            new() { Name = "Сыр пармезан", Count = 15, Measure = "гр", IsAvailable = false },
        ]);

        CurrentProductViewModels = new([
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = false },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 443, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 312, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 33, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 123, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 732, Count = 1, Measure = "шт", IsAvailable = false },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Name = "Холодные напитки “Криспи Гриль”", Price = 1231, Count = 1, Measure = "шт", IsAvailable = true },
        ]);

        foreach (var item in CurrentProductViewModels)
        {

            item.AddToShoppingListCommand = ReactiveCommand.Create(() =>
            {
                ShoppingList.AddictiveViewModels.Add(new(ShoppingList)
                {
                    Name = item.Name,
                    Count = 1,
                    Price = item.Price,
                    Measure = item.Measure,
                    CurrencySymbol = item.CurrencySymbol,
                });
            }, item.WhenAnyValue(x => x.IsAvailable));
        }

        foreach (var item in _fastAddictives)
        {
            item.AddToShoppingListCommand = ReactiveCommand.Create(() =>
            {
                foreach (var currentItem in ShoppingList.CurrentItems.OfType<ShoppingListItemViewModel>())
                {
                    var addictive = new AddictiveForShoppingListItem()
                    {
                        Name = item.Name,
                        Count = 1,
                        Price = item.Price,
                        Measure = item.Measure,
                        ShoppingListViewModel = ShoppingList
                    };
                    addictive.RemoveCommand = ReactiveCommand.Create(() =>
                    {
                        currentItem.Addictives.Remove(addictive);
                    })
                    ;
                    currentItem.Addictives.Add(addictive);
                }
            }, item.WhenAnyValue(x => x.IsAvailable));
        }
    }

    public ShoppingListViewModel ShoppingList
    {
        get;
    }

    public ReadOnlyObservableCollection<AddictiveViewModel> FastAddictives => _fastAddictives;
    private readonly ReadOnlyObservableCollection<AddictiveViewModel> _fastAddictives;

    [Reactive]
    public ReadOnlyObservableCollection<ProductViewModel> CurrentProductViewModels
    {
        get;
        set;
    }
}
