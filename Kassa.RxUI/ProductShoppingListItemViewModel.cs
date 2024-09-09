using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class ProductShoppingListItemViewModel : ReactiveObject, IShoppingListItemVm, IApplicationModelPresenter<ProductDto>, IGuidId
{
    private readonly CompositeDisposable _disposables = [];
    private readonly ObservableCollection<AdditiveShoppingListItemViewModel> _additives = [];
    private readonly IOrderEditVm _orderEditVm;
    private readonly IReceiptService _receiptService;
    private readonly IAdditiveService _additiveService;
    private readonly IApplicationModelManager<ProductDto> _manager;

    public ProductShoppingListItemViewModel(
        ProductShoppingListItemDto productShoppingListItem,
        ProductDto product,
        IApplicationModelManager<ProductDto> manager,
        IOrderEditVm orderEditVm,
        IReceiptService receiptService,
        IAdditiveService additiveService): this(product, manager, orderEditVm, receiptService, additiveService)
    {
        OrderedId = productShoppingListItem.Id;
        Comment = productShoppingListItem.Comment;
    }

    public ProductShoppingListItemViewModel(
        ProductDto product,
        IApplicationModelManager<ProductDto> manager,
        IOrderEditVm orderEditVm,
        IReceiptService receiptService,
        IAdditiveService additiveService)
    {
        _orderEditVm = orderEditVm;
        _additiveService = additiveService;
        _receiptService = receiptService;
        _manager = manager;

        Id = product.Id;
        ProductDto = product;
        Additives = new(_additives);

        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(x =>
            {
                var shoppingList = orderEditVm.ShoppingList;
                if (x)
                {
                    shoppingList.SelectedItems.Add(this);
                }
                else
                {

                    shoppingList.SelectedItems.Remove(this);
                }
            })
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Comment)
            .Select(x => !string.IsNullOrEmpty(x))
            .Subscribe(x => HasAdditiveInfo = x)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveSubtotalSum)
            .Select(x => (x.Item1 * x.Item2) + x.Item3)
            .Subscribe(x => SubtotalSum = x)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveTotalSum)
            .Select(x => ((x.Item1 * x.Item2) + x.Item3) * (1 - Discount))
            .Subscribe(x => TotalSum = x)
            .DisposeWith(_disposables);

        Additives
            .ToObservableChangeSet()
            .AutoRefresh(x => x.Price)
            .AutoRefresh(x => x.Count)
            .ToCollection()
            .Select(list => list.Sum(item => item.Price))
            .Subscribe(x => AddictiveSubtotalSum = x)
            .DisposeWith(_disposables);

        Additives
            .ToObservableChangeSet()
            .ToCollection()
            .Select(list => list.Sum(item => item.Price))
            .Subscribe(x => AddictiveTotalSum = x)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Price, x => x.Count, (price, count) => price*count)
            .ToPropertyEx(this, x => x.PositionPrice)
            .DisposeWith(_disposables);

        RemoveCommand = ReactiveCommand.CreateFromTask(() =>
        {
            return Task.CompletedTask;
        }).DisposeWith(_disposables); ;

        manager.AddPresenter(this)
            .DisposeWith(_disposables);

        Count = 1;

        Update(ProductDto);
    }

    public Guid Id
    {
        get;
    }

    public Guid OrderedId
    {
        get; private set;
    } = Guid.Empty;

    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get; set;
    } = null!;

    [Reactive]
    public bool HasDiscount
    {
        get; set;
    }

    [Reactive]
    public double Discount
    {
        get; set;
    }

    [Reactive]
    public double Count
    {
        get; set;
    }

    [Reactive]
    public string Measure
    {
        get; set;
    } = null!;

    [Reactive]
    public string Name
    {
        get; set;
    } = null!;

    /// <summary>
    /// The base price of a single product unit without any multipliers, additives, or discounts applied.
    /// This value represents the standard price per unit.
    /// </summary>
    [Reactive]
    public double Price
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;

    [Reactive]
    public string? Comment
    {
        get; set;
    }

    public ReadOnlyObservableCollection<AdditiveShoppingListItemViewModel> Additives
    {
        get;
    }

    [Reactive]
    public bool HasAdditiveInfo
    {
        get; set;
    }

    /// <summary>
    /// Be careful with this property. It is not the actual price of the product.
    /// This is the price of the product multiplied by <see cref="Count"/>.
    /// Do not confuse it with the <see cref="Price"/> property, which is the price of a single product.
    /// Do not confuse it with the <see cref="SubtotalSum"/> property, which is the sum of the product and all additives.
    /// Do not confuse it with the <see cref="TotalSum"/> property, which is the sum of the product and all additives after applying discounts.
    /// </summary>
    public extern double PositionPrice
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    [Reactive]
    public double AddictiveSubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double AddictiveTotalSum
    {
        get; set;
    }

    /// <summary>
    /// The total sum of the product's base price multiplied by the quantity, including the cost of any additives.
    /// This value does not include any discounts.
    /// </summary>
    [Reactive]
    public double SubtotalSum
    {
        get; set;
    }

    /// <summary>
    /// The total sum of the product's base price multiplied by the quantity, including the cost of any additives, with all applicable discounts applied.
    /// This value represents the final price to be paid for the product.
    /// </summary>
    [Reactive]
    public double TotalSum
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    public ProductDto ProductDto
    {
        get; private set;
    }

    public void ModelChanged(BuisnessLogic.ApplicationModelManagers.Change<ProductDto> change)
    {
        var model = change.Current;

        Update(model);
    }

    private void Update(ProductDto product)
    {
        Price = product.Price;
        Measure = product.Measure;
        Name = product.Name;
        CurrencySymbol = product.CurrencySymbol;
        ProductDto = product;
    }

    public void Dispose() => _disposables.Dispose();

    /// <summary>
    /// This method adds a additive to the product additive list and spends the ingredients from the storage
    /// </summary>
    /// <remarks>
    /// If you sure that the product is available in storage, you can use <see cref="AddAdditiveUnsafe(ProductDto)"/> method
    /// </remarks>
    public async Task AddAdditive(AdditiveDto additive)
    {
        var storageScope = _orderEditVm.StorageScope;

        var receipt = await _receiptService.GetReceipt(additive.ReceiptId);

        if (receipt is null)
        {
            this.Log().Error("Receipt not found for additive {0}", additive.ReceiptId);
            return;
        }

        if (await storageScope.HasEnoughIngredients(receipt, 1))
        {
            AddAdditiveUnsafe(additive);
        }
    }

    /// <summary>
    /// Use this method only if you are sure that the additive is available in the storage
    /// </summary>
    /// <remarks>
    /// This method does not check the availability of the additive in the storage.
    /// It also does not consume any ingredients.
    /// Use the <see cref="AddAdditive(AdditiveDto)"/> method if you are not sure.
    /// </remarks>
    public void AddAdditiveUnsafe(AdditiveDto additive)
    {
        var additiveShoppingListItemViewModel = new AdditiveShoppingListItemViewModel(additive, _additiveService.RuntimeAdditives, _orderEditVm);

        _additives.Add(additiveShoppingListItemViewModel);
    }

    /// <summary>
    /// This method removes the additive from the product additive list and returns the ingredients to the storage
    /// </summary>
    public async Task RemoveAdditive(AdditiveShoppingListItemViewModel additive)
    {
        var storageScope = _orderEditVm.StorageScope;

        var receipt = await _receiptService.GetReceipt(additive.AdditiveDto.ReceiptId);

        if (receipt is null)
        {
            this.Log().Error("Receipt not found for additive {0}", additive.AdditiveDto.ReceiptId);
            return;
        }

        await storageScope.ReturnIngredients(receipt, additive.Count);

        RemoveAdditiveUnsafe(additive);
    }

    /// <summary>
    /// Use this method only if you are sure that the additive ingredients returned to the storage
    /// </summary>
    /// <remarks>
    /// This method does not return the ingredients to the storage.
    /// To return the ingredients to the storage, use the <see cref="RemoveAdditive(AdditiveShoppingListItemViewModel)"/> method.
    /// </remarks>
    public void RemoveAdditiveUnsafe(AdditiveShoppingListItemViewModel additive)
    {
        _additives.Remove(additive);
    }

    /// <summary>
    /// Use this method only if you are sure that the product is available in the storage
    /// </summary>
    /// <remarks>
    /// This method does not check the availability of the additive/product in the storage.
    /// It also does not consume any ingredients.
    /// You need to spend the ingredients manually.
    /// </remarks>
    public ProductShoppingListItemViewModel CreateCopyUnsafe()
    {
        var copy = new ProductShoppingListItemViewModel(ProductDto, _manager, _orderEditVm, _receiptService, _additiveService)
        {
            Count = Count,
            Comment = Comment,
            IsSelected = IsSelected,
            HasDiscount = HasDiscount,
            Discount = Discount,
            OrderedId = OrderedId
        };

        foreach (var additive in Additives)
        {
            copy.AddAdditiveUnsafe(additive.AdditiveDto);
        }

        return copy;
    }

    public ProductShoppingListItemViewModel CreateCopyWithoutAdditive()
    {
        var copy = new ProductShoppingListItemViewModel(ProductDto, _manager, _orderEditVm, _receiptService, _additiveService)
        {
            Count = Count,
            Comment = Comment,
            IsSelected = IsSelected,
            HasDiscount = HasDiscount,
            Discount = Discount,
            OrderedId = Guid.Empty // Reset the ordered id, because it is a new position
        };

        return copy;
    }
}
