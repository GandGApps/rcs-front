using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.RxUI.Dialogs;
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
            new() { Id = 1, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 2, Name = "", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 3, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = false },
            new() { Id = 4, Name = "Холодные напитки “Криспи Гриль”", Price = 443, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 5, Name = "Холодные напитки “Криспи Гриль”", Price = 312, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 6, Name = "Холодные напитки “Криспи Гриль”", Price = 33, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 7, Name = "Холодные напитки “Криспи Гриль”", Price = 123, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 8, Name = "Холодные напитки “Криспи Гриль”", Price = 732, Count = 1, Measure = "шт", IsAvailable = false },
            new() { Id = 9, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 10, Name = "Холодные напитки “Криспи Гриль”", Price = 1299, Count = 1, Measure = "шт", IsAvailable = true },
            new() { Id = 11, Name = "Холодные напитки “Криспи Гриль”", Price = 1231, Count = 1, Measure = "шт", IsAvailable = true },
        ]);

        foreach (var item in CurrentProductViewModels)
        {

            item.AddToShoppingListCommand = ReactiveCommand.Create(() =>
            {
                ShoppingList.AddictiveViewModels.Add(new(ShoppingList)
                {
                    Id = item.Id,
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

        CreateTotalCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new CommentDialogViewModel(this)).FirstAsync();
        });

        CreatePromocodeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var promo = new PromocodeDialogViewModel(this);
            promo.ApplyCommand.Subscribe(x =>
            {
                DiscountAccesser = x;
            });
            await MainViewModel.DialogOpenCommand.Execute(promo).FirstAsync();

        });

        this.WhenAnyValue(x => x.DiscountAccesser)
            .Subscribe(x =>
            {
                foreach (var item in ShoppingList.AddictiveViewModels)
                {
                    if (x is IDiscountAccesser discountAccesser)
                    {
                        item.HasDiscount = true;
                        if (double.IsNaN(discountAccesser.AccessDicsount(item.Id)))
                        {
                            item.HasDiscount = false;
                            item.Discount = 0;
                        }
                        else
                        {
                            item.Discount = discountAccesser.AccessDicsount(item.Id);
                        }
                        continue;
                    }

                    item.HasDiscount = false;
                    item.Discount = 0;
                }


            });
    }

    public ShoppingListViewModel ShoppingList
    {
        get;
    }

    [Reactive]
    public string TotalComment
    {
        get; set;
    } = string.Empty;

    public ReactiveCommand<Unit, Unit> CreateTotalCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreatePromocodeCommand
    {
        get;
    }

    [Reactive]
    public IDiscountAccesser? DiscountAccesser
    {
        get; set;
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
