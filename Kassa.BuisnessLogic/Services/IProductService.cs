﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;
public interface IProductService : IInitializableService
{
    /// <summary>
    /// <para>
    /// The products that are currently in the runtime. 
    /// </para>
    /// <para>
    /// It's a cache of the products that are in the database.
    /// Every change in the database will be reflected in this cache.
    /// </para>
    /// <para>
    /// Don't use this property to update products. Use <see cref="UpdateProduct(ProductDto)"/> instead. 
    /// Use this property only for connect.
    /// </para>
    /// </summary>
    public IApplicationModelManager<ProductDto> RuntimeProducts
    {
        get;
    }

    public Task UpdateProduct(ProductDto product);
    public Task RemoveProduct(ProductDto product);
    public Task DecreaseProductCount(Guid productId, double count = 1);
    public Task DecreaseProductCount(ProductDto product, double count = 1);
    public Task IncreaseProductCount(Guid productId, double count = 1);
    public Task IncreaseProductCount(ProductDto product, double count = 1);

    public ValueTask<ProductDto?> GetProductById(Guid productId);
}
