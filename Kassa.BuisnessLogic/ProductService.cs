using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
internal sealed class ProductService(IRepository<Product> productRepository, IAdditiveRepository additiveRepository) : IProductService
{
    private bool _isInitialized;
    private bool _isDisposed;

    public SourceCache<Product, int> RuntimeProducts
    {
        get;
    } = new(x => x.Id);

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
            var products = await productRepository.GetAll();

            RuntimeProducts.AddOrUpdate(products);

            _isInitialized = true;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to initialize repository", e);
        }
    }

    public Task DecreaseProductCount(Product product)
    {
        return DecreaseProductCount(product.Id);
    }

    public Task IncreaseProductCount(Product product)
    {
        return IncreaseProductCount(product.Id);
    }

    public async Task DecreaseProductCount(int productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        await UpdateProduct(product with
        {
            Count = product.Count - 1
        });
    }

    public async Task IncreaseProductCount(int productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with id {productId} not found");
        }

        await UpdateProduct(product with
        {
            Count = product.Count + 1
        });

    }

    public async ValueTask<Product?> GetProductById(int productId)
    {
        this.ThrowIfNotInitialized();

        var product = await productRepository.Get(productId);
        if (product is not null)
        {
            UpdateRuntimeProducts(product);
        }

        return product;
    }

    public async Task UpdateProduct(Product product)
    {
        this.ThrowIfNotInitialized();

        await productRepository.Update(product);

        UpdateRuntimeProducts(product);
    }

    private void UpdateRuntimeProducts(Product product)
    {
        RuntimeProducts.Edit(updater =>
        {
            updater.AddOrUpdate(product);
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

    public async Task RemoveProduct(Product product)
    {
        this.ThrowIfNotInitialized();

        await productRepository.Delete(product);

        RuntimeProducts.Remove(product);
    }

    public async ValueTask<IEnumerable<Additive>> GetAdditivesByProductId(int productId)
    {
        this.ThrowIfNotInitialized();

        return await additiveRepository.GetAdditivesByProductId(productId);
    }
}
