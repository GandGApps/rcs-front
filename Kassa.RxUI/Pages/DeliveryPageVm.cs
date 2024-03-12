using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;

namespace Kassa.RxUI.Pages;
public class DeliveryPageVm : PageViewModel
{

    public Guid DeliveryId
    {
        get; set;
    }

    public bool IsPickup
    {
        get; set;
    }

    public bool IsDelivery
    {
        get; set;
    }
}
