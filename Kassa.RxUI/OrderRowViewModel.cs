using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class OrderRowViewModel : ReactiveObject, IApplicationModelPresenter<OrderDto>
{
    private readonly IStreetService _streetService;
    private readonly ICourierService _courierService;
    private readonly IClientService _clientService;

    private readonly CompositeDisposable _disposables = [];

    public OrderDto Order
    {
        get;
    }

    public Guid Id => Order.Id;

    public OrderRowViewModel(OrderDto order, IStreetService streetService, ICourierService courierService, IClientService clientService)
    {
        _streetService = streetService;
        _courierService = courierService;
        _clientService = clientService;

        Order = order;
        Number = Id.ToString("N")[..5];
        Status = order.Status switch
        {
            OrderStatus.Unconfirmed => "Неподтвержден",
            OrderStatus.New => "Новый",
            OrderStatus.InCooking => "Готовится",
            OrderStatus.Ready => "Готов",
            OrderStatus.OnTheWay => "В пути",
            OrderStatus.Completed => "Закрытый",
            OrderStatus.Canceled => "Отменен",
            _ => throw new NotImplementedException()
        };

        TimeToDelivery = order.DeliveryTime.HasValue ? (order.DeliveryTime.Value - DateTime.Now).ToString("hh\\:mm\\:ss") : "00:00:00";

        if (order.StreetId != null)
        {
            var streetOptional = streetService.Streets.Lookup(order.StreetId.Value);
            if (streetOptional.HasValue)
            {
                Address = $"{streetOptional.Value.Name} {order.AddressWithoutStreet}";
            }
            else
            {
                Address = order.AddressWithoutStreet;
            }
        }
        else
        {
            Address = order.AddressWithoutStreet;
        }

        if (order.CourierId is not null)
        {
            var courier = _courierService.RuntimeCouriers.TryGetValue(order.CourierId.Value, out var foundedCourier) ? 
                $"{foundedCourier.FirstName} {foundedCourier.MiddleName} {foundedCourier.LastName}" :
                "Не назначен";
            Courier = courier;
        }
        Courier ??= "Не назначен";

        if (order.ClientId is null)
        {
            throw new NotImplementedException();
        }
        var clientOptional = _clientService.RuntimeClients.Lookup(order.ClientId.Value);

        if (!clientOptional.HasValue)
        {
            throw new NotImplementedException();
        }
        var client = clientOptional.Value;

        Client = $"{client.FirstName} {client.MiddleName} {client.LastName}";

        Comment = order.Comment;
        Value = order.TotalSum.ToString("C2");
    }

    [Reactive]
    public string Number
    {
        get; set;
    }

    [Reactive]
    public string Status
    {
        get; set;
    }

    [Reactive]
    public string TimeToDelivery
    {
        get; set;
    }

    [Reactive]
    public string Address
    {
        get; set;
    }

    [Reactive]
    public string Courier
    {
        get; set;
    }

    [Reactive]
    public string Client
    {
        get; set;
    }

    [Reactive]
    public string? Comment
    {
        get; set;
    }

    [Reactive]
    public string Value
    {
        get; set;
    }

    public void Dispose() => _disposables.Dispose();

    public void ModelChanged(Change<OrderDto> change)
    {
        var order = change.Current;

        Status = order.Status switch
        {
            OrderStatus.Unconfirmed => "Неподтвержден",
            OrderStatus.New => "Новый",
            OrderStatus.InCooking => "Готовится",
            OrderStatus.Ready => "Готов",
            OrderStatus.OnTheWay => "В пути",
            OrderStatus.Completed => "Закрытый",
            OrderStatus.Delivered => "Доставлен",
            OrderStatus.Canceled => "Отменен",
            _ => throw new NotImplementedException()
        };

        TimeToDelivery = order.DeliveryTime.HasValue ? (order.DeliveryTime.Value - DateTime.Now).ToString("hh\\:mm\\:ss") : "00:00:00";

        if (order.StreetId != null)
        {
            var streetOptional = _streetService.Streets.Lookup(order.StreetId.Value);
            if (streetOptional.HasValue)
            {
                Address = $"{streetOptional.Value.Name} {order.AddressWithoutStreet}";
            }
            else
            {
                Address = order.AddressWithoutStreet;
            }
        }
        else
        {
            Address = order.AddressWithoutStreet;
        }

        if (order.CourierId is not null)
        {
            var courier = _courierService.RuntimeCouriers.TryGetValue(order.CourierId.Value, out var foundedCourier) ?
                $"{foundedCourier.FirstName} {foundedCourier.MiddleName} {foundedCourier.LastName}" :
                "Не назначен";
            Courier = courier;
        }
        Courier ??= "Не назначен";

        if (order.ClientId is null)
        {
            throw new NotImplementedException();
        }
        var clientOptional = _clientService.RuntimeClients.Lookup(order.ClientId.Value);

        if (!clientOptional.HasValue)
        {
            throw new NotImplementedException();
        }
        var client = clientOptional.Value;

        Client = $"{client.FirstName} {client.MiddleName} {client.LastName}";

        Comment = order.Comment;
        Value = order.TotalSum.ToString("C2");
    }
}
