﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public class OrderDto
{
    public Guid Id
    {
        get; set;
    }

    public OrderStatus Status
    {
        get; set;
    }

    public DateTime CreatedAt
    {
        get; set;
    }
    public DateTime DeliveryTime
    {
        get; set;
    }

    public string Address
    {
        get; set;
    }

    public CourierDto Courier
    {
        get; set;
    }

    public Guid CourierId
    {
        get; set;
    }

    public IEnumerable<OrderedProductDto> Products
    {
        get; set;
    }
    public string Comment
    {
        get; set;
    }

    public int TotalSum
    {
        get; set;
    }

    public ClientDto Client
    {
        get; set;
    }
}