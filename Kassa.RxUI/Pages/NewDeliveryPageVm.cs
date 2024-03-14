using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class NewDeliveryPageVm : PageViewModel
{

    public NewDeliveryPageVm(ClientViewModel? clientViewModel)
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

    [Reactive]
    public string FullName
    {
        get; set;
    }

    [Reactive]
    public string Phone
    {
        get; set;
    }
}
