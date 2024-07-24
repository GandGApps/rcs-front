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

    public ProductShoppingListItemViewModel(ProductDto product, IApplicationModelManager<ProductDto> manager, IOrderEditVm orderEditVm, IReceiptService receiptService)
    {
        _orderEditVm = orderEditVm;
        _receiptService = receiptService;

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

        this.WhenAnyValue(x => x.AdditiveInfo)
            .Select(x => !string.IsNullOrEmpty(x))
            .Subscribe(x => HasAdditiveInfo = x)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveSubtotalSum)
            .Select(x => (x.Item1 * x.Item2) + x.Item3)
            .Subscribe(x => SubtotalSum = x)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveTotalSum)
            .Select(x => ((x.Item1 * x.Item2) + x.Item3) * (1 -  Discount))
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


        RemoveCommand = ReactiveCommand.CreateFromTask(() => {
            return Task.CompletedTask;
        }).DisposeWith(_disposables); ;

        manager.AddPresenter(this)
            .DisposeWith(_disposables);

        Count = 1;
    }

    public Guid Id
    {
        get;
    }

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
    public string? AdditiveInfo
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

    [Reactive]
    public double SubtotalSum
    {
        get; set;
    }

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
            var additiveVm = new AdditiveShoppingListItemViewModel(additive, _orderEditVm, _receiptService);

            _additives.Add(additiveVm);
        }
    }
}
