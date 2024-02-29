using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class QuantityVolumeDialogVewModel : DialogViewModel
{


    public QuantityVolumeDialogVewModel(ProductShoppingListItemViewModel productShoppingListItemDto)
    {
        ProductShoppingListItem = productShoppingListItemDto;
    }

    public ProductShoppingListItemViewModel ProductShoppingListItem
    {
        get;
    }
}
