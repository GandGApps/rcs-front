using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public record AdditiveDto : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = null!;

    public string CurrencySymbol
    {
        get; set;
    } = null!;

    public double Price
    {
        get; set;
    }
    public string Measure
    {
        get; set;
    } = null!;

    public Guid[] ProductIds
    {
        get; set;
    } = [];

    public int Portion
    {
        get; set;
    }

    public bool IsAvailable
    {
        get; set;
    } = true;

    public bool IsEnoughIngredients
    {
        get; set;
    } = true;

    public Guid ReceiptId
    {
        get; set;
    }
}
