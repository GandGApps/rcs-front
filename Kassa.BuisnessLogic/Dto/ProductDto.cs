using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public record ProductDto : ICategoryItemDto, IDto<Product, ProductDto>
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = null!;


    public string CurrencySymbol
    {
        get; set;
    } = null!;

    public double Price
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public string Measure
    {
        get; set;
    } = null!;

    /// <summary>
    /// <para>
    /// Posible values: 1,2,3
    /// </para>
    /// <para>
    /// If the array is not empty, then it indicates which favorite category the product is in. 
    /// </para>
    /// <para>
    /// There are only 3 favorite categories, this is 1,2,3
    /// </para>
    /// </summary>
    public int[] Favourites
    {
        get; set;
    } = [];

    /// <summary>
    /// Need to be removed, as soon as possible
    /// </summary>
    [Obsolete]
    public string[] Categories
    {
        get; set;
    } = [];


    public string Icon
    {
        get; set;
    } = null!;
    public Guid? CategoryId
    {
        get; set;
    }

    public bool IsAdded
    {
        get; set;
    }

    public bool IsAvailable
    {
        get; set;
    }

    public ICollection<ICategoryItem>? Items
    {
        get;
    } = null;

    [return: NotNullIfNotNull("model")]
    public static ProductDto? FromModel(Product? model) =>  FromProduct(model);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(product))]
    public static ProductDto? FromProduct(Product? product) => product == null ? null : new()
    {
        Id = product.Id,
        Name = product.Name,
        CurrencySymbol = product.CurrencySymbol,
        Price = product.Price,
        Count = product.Count,
        Measure = product.Measure,
        Favourites = product.Favourites,
        Icon = product.Icon,
        CategoryId = product.CategoryId,
    };
    [return: NotNullIfNotNull(nameof(dto))]
    public static Product? ToModel(ProductDto? dto) => ToProduct(dto);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(productDto))]
    public static Product? ToProduct(ProductDto? productDto) => productDto == null ? null : new()
    {
        Id = productDto.Id,
        Name = productDto.Name,
        CurrencySymbol = productDto.CurrencySymbol,
        Price = productDto.Price,
        Count = productDto.Count,
        Measure = productDto.Measure,
        Favourites = productDto.Favourites,
        Icon = productDto.Icon,
        CategoryId = productDto.CategoryId,
    };


}
