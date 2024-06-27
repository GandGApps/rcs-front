using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ProductService : BaseInitializableService, IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IIngridientsService _ingridientsService;
    private readonly IReceiptService _receiptService;
    private readonly HostModelManager<ProductDto> _runtimeProducts = new();

    public ProductService(IRepository<Product> repository, IIngridientsService ingridientsService, IReceiptService receiptService)
    {
        _productRepository = repository;
        _ingridientsService = ingridientsService;
        this._receiptService = receiptService;
    }

    public IApplicationModelManager<ProductDto> RuntimeProducts => _runtimeProducts;

    public async Task DecreaseProductCount(Guid productId, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var product = await _productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var receipt = await _receiptService.GetReceipt(product.ReceiptId);

        if (receipt is not null)
        {
            if (!await _receiptService.HasEnoughIngridients(receipt, count))
            {
                throw new InvalidOperationException($"Product with id {productId} has not enough count");
            }

            await _receiptService.SpendIngridients(receipt, count);
            await CheckAllIngridients((product, receipt));
        }

        
    }

    public Task DecreaseProductCount(ProductDto product, double count = 1) => DecreaseProductCount(product.Id, count);

    public async ValueTask<ProductDto?> GetProductById(Guid productId)
    {
        var product = await _productRepository.Get(productId);

        if (product is null)
        {
            return null;
        }

        var productDto = Mapper.MapProductToProductDto(product);

        RuntimeProducts.AddOrUpdate(productDto);
        return productDto;
    }

    public async Task IncreaseProductCount(Guid productId, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var product = await _productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var receipt = await _receiptService.GetReceipt(product.ReceiptId);

        if (receipt is not null)
        {
            await _receiptService.ReturnIngridients(receipt, count);
            await CheckAllIngridients((product, receipt));

        }
    }

    public Task IncreaseProductCount(ProductDto product, double count = 1) => IncreaseProductCount(product.Id, count);

    public Task RemoveProduct(ProductDto product)
    {
        RuntimeProducts.Remove(product.Id);

        return Task.CompletedTask;
    }

    public Task UpdateProduct(ProductDto product)
    {
        RuntimeProducts.AddOrUpdate(product);

        return Task.CompletedTask;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _ingridientsService.Initialize();
        await _receiptService.Initialize();

        var products = await _productRepository.GetAll();

        foreach (var product in products)
        {

            var productDto = Mapper.MapProductToProductDto(product);

            var receipt = await _receiptService.GetReceipt(product.ReceiptId);

            await HasEnoughIngredients(productDto, receipt, 1);

            RuntimeProducts.AddOrUpdate(productDto);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task CheckAllIngridients((Product product, ReceiptDto receipt)? existingModel)
    {
        var products = RuntimeProducts.Values;

        if (existingModel.HasValue)
        {
            var (product, receipt) = existingModel.Value;
            product.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, 1);
        }


        foreach (var product in products)
        {
            if (existingModel.HasValue && existingModel.Value.product.Id == product.Id)
            {
                continue;
            }

            var receipt = await _receiptService.GetReceipt(product.ReceiptId);

            await HasEnoughIngredients(product, receipt, 1);

            await UpdateProduct(product);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async ValueTask HasEnoughIngredients(ProductDto product, ReceiptDto? receipt, double count)
    {
        if (receipt is null)
        {
            product.IsEnoughIngredients = true;
        }
        else
        {
            product.IsEnoughIngredients = await _receiptService.HasEnoughIngridients(receipt, count);
        }
    }
}
