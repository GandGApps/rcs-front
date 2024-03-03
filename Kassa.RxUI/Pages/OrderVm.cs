using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public class OrderVm : PageViewModel
{
    private readonly Dictionary<object, IEnumerable<ProductViewModel>> _categories = [];

    private readonly IOrderService _order;
    private readonly ICashierService _cashierService;

    private Action<bool>? _isMultiSelectSetter;

    public OrderVm(IOrderService order, ICashierService cashierService)
    {
        _order = order;
        _cashierService = cashierService;

        CreateTotalCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this)
            {
                Comment = TotalComment
            };

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished)
            {
                await _order!.WriteTotalComment(dialog.Comment);
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
            var dialog = await MainViewModel.DialogOpenCommand.Execute(promo).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SearchAddictiveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var additiveService = await Locator.GetInitializedService<IAdditiveService>();
            var addictiveDialog = new SearchAddictiveDialogViewModel(additiveService, order);
            var dialog = await MainViewModel.DialogOpenCommand.Execute(addictiveDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        SelectFavouriteCommand = ReactiveCommand.CreateFromTask(async (int x) =>
        {
            if (_order == null)
            {
                return;
            }
            await _order.SelectFavourite(x);
        });

        SelectRootCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (_order == null)
            {

                return;
            }
            await _order.SelectRootCategory();
        });

        SearchProductCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var searchProductDialog = new SearchProductDialogViewModel();
            var dialog = await MainViewModel.DialogOpenCommand.Execute(searchProductDialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        CreateCommentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new CommentDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();

            if (dialog.IsPublished && _order is not null)
            {
                await _order.WriteCommentToSelectedItems(dialog.Comment);
            }
        });

        OpenMoreDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            var dialog = new MoreCashierDialogViewModel(this);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        OpenDiscountsAndSurchargesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new DiscountsAndSurchargesDialogViewModel();

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        GoToPaymentCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var payment = await _cashierService.CreatePayment(order);
            var paymentPage = new CashierPaymentPageVm(payment);

            MainViewModel.GoToPageCommand.Execute(paymentPage).Subscribe();
        });

        OpenQuantityVolumeDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (ShoppingListItems == null)
            {
                return;
            }
            var shoppingListItem = ShoppingListItems.FirstOrDefault(x => x.IsSelected);

            if (shoppingListItem == null)
            {
                return;
            }

            var dialog = new QuantityVolumeDialogVewModel(shoppingListItem);

            await MainViewModel.DialogOpenCommand.Execute(dialog).FirstAsync();

            await dialog.WaitDialogClose();
        });

        GoToAllOrdersCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var allOrdersPage = new AllOrdersPageVm();
            await MainViewModel.GoToPageCommand.Execute(allOrdersPage).FirstAsync();
        });
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        ShoppingList = new(_order);

        _order.BindSelectedCategoryItems(out var categoryItems)
                       .DisposeWith(disposables);

        _order.BindShoppingListItems(x => new ProductShoppingListItemViewModel(x, _order), out var shoppingListItems)
                       .DisposeWith(disposables);

        _order.BindAdditivesForSelectedProduct(x => new AdditiveViewModel(x, _order), out var fastAdditives)
                       .DisposeWith(disposables);


        _isMultiSelectSetter = x => _order.IsMultiSelect = x;
        IsMultiSelect = _order.IsMultiSelect;

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

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => ShoppingList.Subtotal = x)
                         .DisposeWith(disposables);

        return ValueTask.CompletedTask;
    }

    public bool IsMultiSelect
    {

        get => ShoppingList != null && ShoppingList.IsMultiSelect;
        set
        {
            if (ShoppingList == null)
            {
                return;
            }

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
    public ShoppingListViewModel? ShoppingList
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get; private set;
    }

    [Reactive]
    public ReadOnlyObservableCollection<AdditiveViewModel>? FastAdditives
    {
        get; private set;
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

    public ReactiveCommand<int, Unit> SelectFavouriteCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> SelectRootCategoryCommand
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

    public ReactiveCommand<Unit, Unit> GoToPaymentCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoToAllOrdersCommand
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
        get; private set;
    }
    public ReactiveCommand<Unit, Unit> OpenQuantityVolumeDialogCommand
    {
        get;
    }
}
