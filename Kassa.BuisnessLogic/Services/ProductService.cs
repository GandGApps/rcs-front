using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
internal sealed class ProductService(IRepository<Product> productRepository, IAdditiveRepository additiveRepository) : IProductService, IRuntimeDtoProvider<ProductDto, Guid, Product>
{
    private bool _isInitialized;
    private bool _isDisposed;

    public SourceCache<ProductDto, Guid> RuntimeProducts
    {
        get;
    } = new(x => x.Id);

    public bool IsInitialized => _isInitialized;

    public bool IsDisposed => _isDisposed;

    SourceCache<ProductDto, Guid> IRuntimeDtoProvider<ProductDto, Guid, Product>.RuntimeDtos => RuntimeProducts;
    IRepository<Product> IRuntimeDtoProvider<ProductDto, Guid, Product>.Repository => productRepository;


    public async ValueTask Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        try
        {
            var products = await productRepository.GetAll();

            RuntimeProducts.AddOrUpdate(products.Select(x => ToProductDto(x)));

            _isInitialized = true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to initialize service", e);
        }
    }

    public Task DecreaseProductCount(ProductDto product)
    {
        return DecreaseProductCount(product.Id);
    }

    public Task IncreaseProductCount(ProductDto product)
    {
        return IncreaseProductCount(product.Id);
    }

    public async Task DecreaseProductCount(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        product.Count--;

        await productRepository.Update(product);

        await UpdateProduct(ToProductDto(product));
    }

    public async Task IncreaseProductCount(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        product.Count++;

        await productRepository.Update(product);

        await UpdateProduct(ToProductDto(product));

    }

    public async ValueTask<ProductDto?> GetProductById(Guid productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);
        var productDto = ToProductDto(product);

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

        product = ProductDto.ToProduct(productDto);

        await productRepository.Update(product);

        UpdateRuntimeProducts(productDto);
    }

    private void UpdateRuntimeProducts(ProductDto productDto)
    {
        RuntimeProducts.Edit(updater =>
        {
            updater.AddOrUpdate(productDto);
        });
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

        RuntimeProducts.Remove(productDto);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(product))]
    private static ProductDto? ToProductDto(Product? product) => ProductDto.FromProduct(product);

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
