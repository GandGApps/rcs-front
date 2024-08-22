using System;
using System.Collections.Generic;
using System.Text;

namespace Kassa.DataAccess.Model;
public class Order : IGuidId
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

    public Guid? CourierId
    {
        get; set;
    }

    public IEnumerable<OrderedProduct> Products
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

    public bool IsForHere
    {
        get; set;
    }

    public PaymentInfo? PaymentInfo
    {
        get; set;
    }

    public Guid? PaymentInfoId
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

    public string CardType
    {
        get; set;
    }
}
