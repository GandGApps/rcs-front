using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData.Aggregation;
using Kassa.BuisnessLogic;
using Splat;
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Pages;
using System.Reactive.Disposables;

namespace Kassa.RxUI;
public class ShoppingListViewModel : BaseViewModel
{
    private readonly ObservableCollection<ProductShoppingListItemViewModel> _productShoppingListItems = [];
    private readonly IOrderEditVm _orderEditVm;
    private readonly IProductService _productService;
    private readonly IReceiptService _receiptService;

    public ShoppingListViewModel(IOrderEditVm orderEditVm, IProductService productService, IReceiptService receiptService)
    {
        _orderEditVm = orderEditVm;
        _productService = productService;
        _receiptService = receiptService;

        ProductShoppingListItems = new(_productShoppingListItems);

        IncreaseSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var selectedProducts = orderEditVm.ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>();
            var storageScope = orderEditVm.StorageScope;

            foreach (var product in selectedProducts)
            {
                var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

                if (receipt is null)
                {
                    this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                    continue;
                }

                if (await storageScope.HasEnoughIngredients(receipt, 1))
                {
                    await storageScope.SpendIngredients(receipt, 1);

                    product.Count++;
                }
            }
        });

        DecreaseSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var selectedProducts = orderEditVm.ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>();
            var storageScope = orderEditVm.StorageScope;

            foreach (var product in selectedProducts)
            {
                var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

                if (receipt is null)
                {
                    this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                    continue;
                }

                if (product.Count > 0)
                {
                    await storageScope.ReturnIngredients(receipt, 1);

                    product.Count--;
                }
            }
        });

        RemoveSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var selectedProducts = orderEditVm.ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>();
            var storageScope = orderEditVm.StorageScope;

            foreach (var product in selectedProducts)
            {
                var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

                if (receipt is null)
                {
                    this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                    continue;
                }

                await storageScope.ReturnIngredients(receipt, product.Count);

                _productShoppingListItems.Remove(product);
            }
        });

        this.WhenAnyValue(x => x.Subtotal)
            .Select(x => x - Discount)
            .Subscribe(x => Total = x)
            .DisposeWith(InternalDisposables);
    }

    public ReactiveCommand<Unit, Unit> IncreaseSelectedCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> DecreaseSelectedCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> RemoveSelectedCommand
    {
        get;
    }

    public bool IsMultiSelect
    {
        get; set;
    }

    [Reactive]
    public double Discount
    {
        get; set;
    }

    public ObservableCollection<IShoppingListItemVm> SelectedItems
    {
        get;
    } = [];


    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel> ProductShoppingListItems
    {
        get;
    }

    [Reactive]
    public double Subtotal
    {
        get; set;
    }

    [Reactive]
    public double Total
    {
        get; set;
    }

    public async Task AddProductShoppingListItem(ProductDto product)
    {
        var receipt = await _receiptService.GetReceipt(product.ReceiptId);

        if (receipt is null)
        {
            this.Log().Error("Receipt not found for product {0}", product.ReceiptId);
            return;
        }

        if (await _orderEditVm.StorageScope.HasEnoughIngredients(receipt, 1))
        {
            await _orderEditVm.StorageScope.SpendIngredients(receipt, 1);
        }
        else
        {
            this.Log().Error("Not enough ingredients for product {0}", product.ReceiptId);
            return;
        }

        var productShoppingListItemVm = new ProductShoppingListItemViewModel(product, _productService.RuntimeProducts, _orderEditVm, _receiptService);
        _productShoppingListItems.Add(productShoppingListItemVm);
    }

    public async Task AddAdditiveToSelectedProducst(AdditiveDto additive)
    {
        var selectedProducts = _orderEditVm.ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>();

        foreach (var product in selectedProducts)
        {
            var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

            if (receipt is null)
            {
                this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                continue;
            }

            if (await _orderEditVm.StorageScope.HasEnoughIngredients(receipt, 1))
            {
                await _orderEditVm.StorageScope.SpendIngredients(receipt, 1);
            }
            else
            {
                this.Log().Error("Not enough ingredients for product {0}", product.ProductDto.ReceiptId);
                continue;
            }

            await product.AddAdditive(additive);
        }
    }
}
