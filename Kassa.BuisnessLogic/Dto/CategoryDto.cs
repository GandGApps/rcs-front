using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public class CategoryDto : ICategoryItemDto
{
    public int Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    }

    public string? Icon
    {
        get; set;
    }

    public bool HasIcon
    {
        get; set;
    }

    public int? CategoryId
    {
        get; set;
    }

    public virtual ICollection<Category> Categories
    {
        get; set;
    }

    public virtual ICollection<Product> Products
    {
        get; set;
    }

    public ICollection<ICategoryItem>? Items
    {
        get;
    }

    public int[] Favourites
    {
        get;
    } = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(category))]
    public static CategoryDto? FromCategory(Category? category) => category == null ? null : new()
    {

        Id = category.Id,
        Name = category.Name,
        Icon = category.Icon,
        CategoryId = category.CategoryId,
        Categories = category.Categories,
        Products = category.Products,
    };

    public override string ToString()
    {
        return $"'{Name}'";
    }
}
