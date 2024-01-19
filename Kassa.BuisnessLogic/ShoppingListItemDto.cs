using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public record ShoppingListItemDto
{

    public ShoppingListItemDto(Product product)
    {
    }

    public int Id
    {
        get; 
    }

    public string Name
    {
        get;
    }
}
