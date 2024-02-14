using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Dto;
public record AdditiveDto : IDto<Additive, AdditiveDto>
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

    public double Count
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
    }

    public int Portion
    {
        get; set;
    }


    [return: NotNullIfNotNull(nameof(model))]
    public static AdditiveDto? FromModel(Additive? model) => model is null ? null : new AdditiveDto
    {
        Id = model.Id,
        Name = model.Name,
        CurrencySymbol = model.CurrencySymbol,
        Price = model.Price,
        Count = model.Count,
        Measure = model.Measure,
        ProductIds = model.ProductIds,
        Portion = model.Portion,
    };

    [return: NotNullIfNotNull(nameof(dto))]
    public static Additive? ToModel(AdditiveDto? dto) => dto is null ? null : new Additive
    {
        Id = dto.Id,
        Name = dto.Name,
        CurrencySymbol = dto.CurrencySymbol,
        Price = dto.Price,
        Count = dto.Count,
        Measure = dto.Measure,
        ProductIds = dto.ProductIds,
        Portion = dto.Portion,
    };
}
