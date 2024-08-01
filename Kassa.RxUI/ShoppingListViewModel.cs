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

public sealed class ShoppingListViewModel : BaseViewModel
{
    private readonly ObservableCollection<ProductShoppingListItemViewModel> _productShoppingListItems = [];
    private readonly IOrderEditVm _orderEditVm;
    private readonly IProductService _productService;
    private readonly IReceiptService _receiptService;
    private readonly IAdditiveService _additiveService;

    public ShoppingListViewModel(IOrderEditVm orderEditVm, IProductService productService, IReceiptService receiptService, IAdditiveService additiveService)
    {
        _orderEditVm = orderEditVm;
        _productService = productService;
        _receiptService = receiptService;
        _additiveService = additiveService;

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

    /// <summary>
    /// This method adds a product to the shopping list and spends the ingredients from the storage
    /// </summary>
    /// <remarks>
    /// If you sure that the product is available in storage, you can use <see cref="AddProductShoppingListItemUnsafe(ProductDto)"/> method
    /// </remarks>
    public async Task AddProductShoppingListItem(ProductDto product)
    {

        if (product.ReceiptId != Guid.Empty)
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
        }

        // I'm sure this is safe because I have checked that the product is available in storage
        AddProductShoppingListItemUnsafe(product);
    }

    /// <summary>
    /// Use this method only if you are sure that the product is available in storage.
    /// </summary>
    /// <remarks>
    /// This method does not check if the product is available in storage.
    /// It also does not consume any ingredients.
    /// Use the <see cref="AddProductShoppingListItem(ProductDto)"/> method if you are not sure.
    /// </remarks>
    public void AddProductShoppingListItemUnsafe(ProductDto product)
    {
        var productShoppingListItemVm = new ProductShoppingListItemViewModel(product, _productService.RuntimeProducts, _orderEditVm, _receiptService, _additiveService);
        AddProductShoppingListItemUnsafe(productShoppingListItemVm);
    }

    /// <summary>
    /// Use this method only if you are sure that the product is available in storage.
    /// </summary>
    /// <remarks>
    /// This method does not check if the product is available in storage.
    /// It also does not consume any ingredients.
    /// Use the <see cref="AddProductShoppingListItem(ProductDto)"/> method if you are not sure.
    /// </remarks>
    public void AddProductShoppingListItemUnsafe(ProductShoppingListItemViewModel product)
    {
        _productShoppingListItems.Add(product);
    }

    public async Task AddAdditiveToSelectedProducst(AdditiveDto additive)
    {
        var selectedProducts = _orderEditVm.ShoppingList.SelectedItems.OfType<ProductShoppingListItemViewModel>();
        var receipt = await _receiptService.GetReceipt(additive.ReceiptId);

        if (receipt is null)
        {
            this.Log().Error("Receipt not found for product {0}", additive.ReceiptId);
            return;
        }

        foreach (var product in selectedProducts)
        {
            if (await _orderEditVm.StorageScope.HasEnoughIngredients(receipt, 1))
            {
                await _orderEditVm.StorageScope.SpendIngredients(receipt, 1);
            }
            else
            {
                this.Log().Error("Not enough ingredients for product {0}", additive.ReceiptId);
                continue;
            }

            // I'm sure this is safe because I have checked that the additive is available in storage
            product.AddAdditiveUnsafe(additive);
        }
    }

    public async Task IncreaseProductShoppingListItemViewModel(ProductShoppingListItemViewModel product, double count)
    {
        if (product.ProductDto.ReceiptId != Guid.Empty)
        {
            var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

            if (receipt is null)
            {
                this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                return;
            }

            if (await _orderEditVm.StorageScope.HasEnoughIngredients(receipt, count))
            {
                await _orderEditVm.StorageScope.SpendIngredients(receipt, count);

                product.Count += count;
            }
        }
        else
        {
            product.Count += count;
        }

       
    }

    public async Task DecreaseProductShoppingListItemViewModel(ProductShoppingListItemViewModel product, double count)
    {
        if (product.ProductDto.ReceiptId != Guid.Empty)
        {
            var receipt = await _receiptService.GetReceipt(product.ProductDto.ReceiptId);

            if (receipt is null)
            {
                this.Log().Error("Receipt not found for product {0}", product.ProductDto.ReceiptId);
                return;
            }

            if (product.Count >= count)
            {
                await _orderEditVm.StorageScope.ReturnIngredients(receipt, count);

                product.Count -= count;
            }
        }
        else
        {
            product.Count -= count;
        }

        
    }
}
