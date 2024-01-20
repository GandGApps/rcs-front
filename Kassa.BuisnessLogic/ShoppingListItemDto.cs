﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Price = product.Price;
        CurrencySymbol = product.CurrencySymbol;
        Measure = product.Measure;
        Count = 1;
        Discount = 1;

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

    public double Count
    {
        get; init;
    }
    public string CurrencySymbol
    {
        get;
    }
    public double Discount
    {
        get;
        init;
    }
    public bool HasDiscount
    {
        get;
        set;
    }
    public string Measure
    {
        get;
    }
    public double Price
    {
        get;
        init;
    }
    public double SubtotalSum => Price * Count;
    public double TotalSum => SubtotalSum * Discount;

    public bool HasAdditives => Additives.Count > 0;

    public bool HasAdditiveInfo => !string.IsNullOrWhiteSpace(AdditiveInfo);

    public List<Additive> Additives
    {
        get;
    } = [];

    public string? AdditiveInfo
    {
        get; init;
    } 
}
