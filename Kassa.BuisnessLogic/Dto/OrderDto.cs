using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public class OrderDto : IModel
{
    public Guid Id
    {
        get; set;
    }

    public string Problem
    {
        get; set;
    } = string.Empty;

    public OrderStatus Status
    {
        get; set;
    }

    public DateTime CreatedAt
    {
        get; set;
    }

    public DateTime? DeliveryTime
    {
        get; set;
    }

    public CourierDto? Courier
    {
        get; set;
    }

    public Guid? CourierId
    {
        get; set;
    }

    public IEnumerable<OrderedProductDto> Products
    {
        get; set;
    } = [];

    public string? Comment
    {
        get; set;
    } = string.Empty;

    public double TotalSum
    {
        get; set;
    }

    public double SubtotalSum
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public ClientDto? Client
    {
        get; set;
    }

    public bool IsDelivery
    {
        get; set;
    }

    public Guid? ClientId
    {
        get; set;
    }

    public string LastName
    {
        get; set;
    } = string.Empty;

    public string Phone
    {
        get; set;
    } = string.Empty;

    public string Card
    {
        get; set;
    } = string.Empty;

    public string Miscellaneous
    {
        get; set;
    } = string.Empty;

    public string House
    {
        get; set;
    } = string.Empty;

    public string Building
    {
        get; set;
    } = string.Empty;

    public string Entrance
    {
        get; set;
    } = string.Empty;

    public string Floor
    {
        get; set;
    } = string.Empty;

    public string Apartment
    {
        get; set;
    } = string.Empty;

    public string Intercom
    {
        get; set;
    } = string.Empty;

    public string AddressNote
    {
        get; set;
    } = string.Empty;

    public bool IsPickup
    {
        get; set;
    }

    public Guid? StreetId
    {
        get; set;
    }

    public Guid? DistrictId
    {
        get; set;
    }

    public string FirstName
    {
        get; set;
    } = string.Empty;

    public string MiddleName
    {
        get; set;
    } = string.Empty;

    public bool IsOutOfTurn
    {
        get; set;
    }

    public bool IsProblematicDelivery
    {
        get; set;
    }

    public PaymentInfoDto? PaymentInfo
    {
        get; set;
    }

    public bool IsForHere
    {
        get; set;
    }

    public Guid PaymentInfoId
    {
        get; set;
    }

    public Guid ShiftId
    {
        get; set;
    }

    public Guid CashierShiftId
    {
        get; set;
    }

    public string AddressWithoutStreet => $"{House} {Building} {Entrance} {Floor} {Apartment}";
}