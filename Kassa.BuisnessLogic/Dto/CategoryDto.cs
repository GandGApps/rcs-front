using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class CategoryDto : ICategoryItemDto, ICategoryDto, IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = string.Empty;

    public int Image
    {
        get; set;
    }

    public bool HasIcon
    {
        get; set;
    }

    public Guid? CategoryId
    {
        get; set;
    }

    public virtual ICollection<Category> Categories
    {
        get; set;
    } = [];

    public virtual ICollection<Product> Products
    {
        get; set;
    } = [];

    public ICollection<ICategoryItem>? Items
    {
        get;
    }

    public int[] Favourites
    {
        get;
    } = [];

    public string? Color
    {
        get; set;
    }

    public override string ToString()
    {
        return $"'{Name}'";
    }
}
