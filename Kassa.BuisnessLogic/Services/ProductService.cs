using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal sealed class ProductService(
    IRepository<Product> productRepository,
    IReceiptService receiptService) : IProductService
{
    private bool _isInitialized;
    private bool _isDisposed;

    public IApplicationModelManager<ProductDto> RuntimeProducts
    {
        get;
    } = new HostModelManager<ProductDto>();

    public bool IsInitialized => _isInitialized;

    public bool IsDisposed => _isDisposed;

    public async ValueTask Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        try
        {
            await receiptService.Initialize();

            var products = await productRepository.GetAll();

            foreach (var product in products)
            {

                var productDto = Mapper.MapProductToProductDto(product);
                var receipt = await receiptService.GetReceipt(product.ReceiptId);

                if (receipt is null)
                {
                    throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
                }

                productDto.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);

                RuntimeProducts.AddOrUpdate(productDto);
            }

            _isInitialized = true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to initialize service", e);
        }
    }

    public Task DecreaseProductCount(ProductDto product, double count = 1)
    {
        return DecreaseProductCount(product.Id, count);
    }

    public Task IncreaseProductCount(ProductDto product, double count = 1)
    {
        return IncreaseProductCount(product.Id, count);
    }

    public async Task DecreaseProductCount(Guid productId, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var receipt = await receiptService.GetReceipt(product.ReceiptId);

        if (receipt is null)
        {
            throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
        }

        if (!await receiptService.HasEnoughIngridients(receipt, count))
        {
            throw new InvalidOperationException($"Product with id {productId} has not enough count");
        }

        await receiptService.SpendIngridients(receipt, count);
        await CheckAllIngridients((product,receipt));

        await productRepository.Update(product);

        await UpdateProduct(Mapper.MapProductToProductDto(product));
    }

    public async Task IncreaseProductCount(Guid productId, double count = 1)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        var receipt = await receiptService.GetReceipt(product.ReceiptId);

        if (receipt is null)
        {
            throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
        }

        await receiptService.ReturnIngridients(receipt, count);
        await CheckAllIngridients((product, receipt));

        await productRepository.Update(product);

        await UpdateProduct(Mapper.MapProductToProductDto(product));

    }

    public async ValueTask<ProductDto?> GetProductById(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            return null;
        }

        var productDto = Mapper.MapProductToProductDto(product);

        if (productDto is not null)
        {
            UpdateRuntimeProducts(productDto);
        }

        return productDto;
    }

    public async Task UpdateProduct(ProductDto productDto)
    {
        this.ThrowIfNotInitialized();

        var product = await GetProductOrThrow(productDto, productRepository);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productDto.Id} not found");
        }

        product = Mapper.MapProductDtoToProduct(productDto);

        await productRepository.Update(product);

        UpdateRuntimeProducts(productDto);
    }

    private void UpdateRuntimeProducts(ProductDto productDto)
    {
        RuntimeProducts.AddOrUpdate(productDto);
    }

    public void Dispose()
    {
        _isDisposed = true;
        RuntimeProducts.Dispose();
    }
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public async Task RemoveProduct(ProductDto productDto)
    {
        this.ThrowIfNotInitialized();

        var product = await GetProductOrThrow(productDto, productRepository);

        await productRepository.Delete(product);

        RuntimeProducts.Remove(productDto.Id);
    }

    private async Task CheckAllIngridients((Product product, ReceiptDto receipt)? existingModel)
    {
        var products = RuntimeProducts.Values;

        if (existingModel.HasValue)
        {
            var (product, receipt) = existingModel.Value;
            product.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);
        }


        foreach (var product in products)
        {
            if (existingModel.HasValue && existingModel.Value.product.Id == product.Id)
            {
                continue;
            }

            var receipt = await receiptService.GetReceipt(product.ReceiptId);

            if (receipt is null)
            {
                throw new InvalidOperationException($"Receipt with id {product.ReceiptId} not found");
            }

            product.IsEnoughIngredients = await receiptService.HasEnoughIngridients(receipt, 1);

            await UpdateProduct(product);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task<Product> GetProductOrThrow(Guid id, IRepository<Product> repository)
    {
        var product = await repository.Get(id);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {id} not found");
        }

        return product;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task<Product> GetProductOrThrow(ProductDto productDto, IRepository<Product> repository) => GetProductOrThrow(productDto.Id, repository);
}
