using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic;
using Kassa.DataAccess;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public class CashierVm : PageViewModel
{
    private readonly Dictionary<object, IEnumerable<ProductViewModel>> _categories = [];

    private ICashierService? _cashierService;

    private Action<bool>? _isMultiSelectSetter;

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
            var dialog = new CommentDialogViewModel(this);

            await mainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished)
            {
                TotalComment = dialog.Comment;
            }
        });

        CreatePromocodeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var promo = new PromocodeDialogViewModel(this);
            promo.ApplyCommand.Subscribe(x =>
            {
                DiscountAccesser = x;
            });
            var dialog = await mainViewModel.DialogOpenCommand.Execute(promo).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SearchAddictiveCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var addictiveDialog = new SearchAddictiveDialogViewModel(mainViewModel);
            var dialog = await mainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (addictiveDialog.SelectedAddictve is not null)
            {

                foreach (var item in ShoppingList.CurrentItems.OfType<ShoppingListItemViewModel>())
                {

                    var addictive = new AddictiveForShoppingListItem()
                    {
                        Name = addictiveDialog.SelectedAddictve.Name,
                        Count = 1,
                        Price = addictiveDialog.SelectedAddictve.Price,
                        Measure = addictiveDialog.SelectedAddictve.Measure,
                        ShoppingListViewModel = ShoppingList
                    };
                    addictive.RemoveCommand = ReactiveCommand.Create(() =>
                    {
                        item.Addictives.Remove(addictive);
                    });
                    item.Addictives.Add(addictive);
                }
            }
        });

        SelectCategoryCommand = ReactiveCommand.Create<string>(x =>
        {
            Category = x;
        });

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var searchProductDialog = new SearchProductDialogViewModel();
            var dialog = await mainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();

            var product = searchProductDialog.SelectedProduct;

            if (product is null)
            {
                return;
            }

            ShoppingList.AddictiveViewModels.Add(new ShoppingListItemViewModel(ShoppingList, product));
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
               {

                   var dialog = new CommentDialogViewModel(this);

                   await mainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

                   await dialog.WaitDialogClose();

                   if (dialog.IsPublished)
                   {

                       foreach (var item in ShoppingList.CurrentItems.OfType<ShoppingListItemViewModel>())
                       {

                           item.AddictiveInfo = dialog.Comment;
                       }
                   }
               });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var dialog = new MoreCashierDialogViewModel(this);

            await mainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        OpenDiscountsAndSurchargesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new DiscountsAndSurchargesDialogViewModel();

            await mainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });
    }

    protected override sealed void OnActivated(CompositeDisposable disposables)
    {
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


            })
            .DisposeWith(disposables);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        _cashierService = await GetInitializedService<ICashierService>();

        _cashierService.BindSelectedCategoryItems(out var categoryItems)
                       .DisposeWith(disposables);

        _cashierService.BindShoppingListItems(out var shoppingListItems)
                       .DisposeWith(disposables);

        IsMultiSelect = _cashierService.IsMultiSelect;
        _isMultiSelectSetter = x => _cashierService.IsMultiSelect = x;

        CurrentCategoryItems = categoryItems;
        ShoppingListItems = shoppingListItems;
    }

    public bool IsMultiSelect
    {

        get => ShoppingList.IsMultiSelect;
        set
        {
            if (value == ShoppingList.IsMultiSelect)
            {
                return;
            }
            ShoppingList.IsMultiSelect = value;
            _isMultiSelectSetter?.Invoke(value);

            this.RaisePropertyChanged();
        }
    }

    public ShoppingListViewModel ShoppingList
    {
        get;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ShoppingListItemDto> ShoppingListItems
    {
        get; set;
    }

    [Reactive]
    public string? TotalComment
    {
        get; set;
    }

    [Reactive]
    public string? Category
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> CreateTotalCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SearchAddictiveCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreatePromocodeCommand
    {
        get;
    }

    public ReactiveCommand<string, Unit> SelectCategoryCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SearchProductCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CreateCommentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenMoreDialogCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDiscountsAndSurchargesDialog
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
    public ReadOnlyObservableCollection<ICategoryItem>? CurrentCategoryItems
    {
        get; set;
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        base.Dispose(disposing);

        _cashierService = null;
    }

    protected async override ValueTask DisposeAsyncCore()
    {
        if (IsDisposed)
        {
            return;
        }

        await _cashierService!.DisposeAsync();
    }
}
