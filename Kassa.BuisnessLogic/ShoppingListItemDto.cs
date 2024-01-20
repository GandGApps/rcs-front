using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public record ShoppingListItemDto : IShoppingListItemDto
{

    public ShoppingListItemDto(Product product, bool isSelected = true)
    {
        ItemId = product.Id;
        Name = product.Name;

        IsSelected = isSelected;
    }

    public int ItemId
    {
        get;
    }

    public Guid Id
    {
        get; init;
    }

    public string Name
    {
        get;
    }

    public bool IsSelected
    {
        get;
    }
}
