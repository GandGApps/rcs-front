using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;

namespace Kassa.RxUI.Pages;
public class DeliveryPageVm : PageViewModel
{

    public DeliveryPageVm(ClientViewModel? clientViewModel)
    {
        DeliveryId = Guid.NewGuid();
        Client = clientViewModel;
    }

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

    public ClientViewModel? Client
    {
        get;
    }
}
