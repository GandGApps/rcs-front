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
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
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
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await mainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished && _cashierService is not null)
            {
                await _cashierService.WriteCommentToSelectedItems(dialog.Comment);
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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        _cashierService = await GetInitializedService<ICashierService>();
        ShoppingList = new(_cashierService);

        _cashierService.BindSelectedCategoryItems(out var categoryItems)
                       .DisposeWith(disposables);

        _cashierService.BindShoppingListItems(x => new ProductShoppingListItemViewModel(x),out var shoppingListItems)
                       .DisposeWith(disposables);

        _cashierService.BindAdditivesForSelectedProduct(x => new AdditiveViewModel(x), out var fastAdditives)
                       .DisposeWith(disposables);

        _isMultiSelectSetter = x => _cashierService.IsMultiSelect = x;
        IsMultiSelect = _cashierService.IsMultiSelect;

        CurrentCategoryItems = categoryItems;
        ShoppingListItems = shoppingListItems;
        FastAdditives = fastAdditives;

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

    [Reactive]
    public ShoppingListViewModel ShoppingList
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel> ShoppingListItems
    {
        get; set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<AdditiveViewModel> FastAdditives
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

    [Reactive]
    public ReadOnlyObservableCollection<ICategoryItemDto>? CurrentCategoryItems
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
