using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using ReactiveUI;
using Splat;

namespace Kassa.RxUI;
public sealed class ServiceOrderRowViewModel : ReactiveObject, IGuidId, IApplicationModelPresenter<OrderDto>
{
    private readonly IShiftService _shiftService;
    private readonly IProductService _productService;

    public Guid Id
    {
        get;
    }

    public OrderDto? Order
    {
        get; private set;
    }

    public ServiceOrderRowViewModel(OrderEditDto order, IShiftService shiftService, IProductService productService, ICashierService cashierService)
    {
        _shiftService = shiftService;
        _productService = productService;

        Debug.Assert(shiftService.CurrentShift.Value != null);

        Id = order.Id;
        Number = order.Id.GuidToPrettyInt();
        Time = order.CreatedAt.ToString("dd.MM.yyyy | HH:mm");
        CashierName = shiftService.CurrentShift.Value.Member.Name;
        Amount = order.Products.Sum(x => x.TotalSum + x.Additives.Sum(x => x.TotalSum)).ToString("F2");
        Composition = string.Join(", ", order.Products.Take(4).Select(x =>
        {
            var productDto = productService.RuntimeProducts.TryGetValue(x.ItemId, out var product) ? product : null;

            if (productDto is null)
            {
                this.Log().Error($"Product with id {x.ItemId} not found");
            }

            return productDto?.Name ?? "Неизвестный продукт";
        }));

        ExternalNumber = "???";
        ReceiptNumber = "???";
    }

    public ServiceOrderRowViewModel(OrderDto order, IShiftService shiftService, IProductService productService, ICashierService cashierService)
    {
        Order = order;
        _shiftService = shiftService;
        _productService = productService;

        Debug.Assert(shiftService.CurrentShift.Value != null);

        Id = order.Id;
        Number = order.Id.GuidToPrettyInt();
        Time = order.CreatedAt.ToString("dd.MM.yyyy | HH:mm");
        CashierName = shiftService.CurrentShift.Value.Member.Name;
        Amount = order.Products.Sum(x => x.TotalPrice + x.Additives.Sum(x => x.TotalPrice)).ToString("F2");
        Composition = string.Join(", ", order.Products.Take(4).Select(x =>
        {
            var productDto = productService.RuntimeProducts.TryGetValue(x.ProductId, out var product) ? product : null;


            if (productDto is null)
            {
                this.Log().Error($"Product with id {x.ProductId} not found");
            }

            return productDto?.Name ?? "Неизвестный продукт";
        }));

        ExternalNumber = "???";
        ReceiptNumber = "???";
    }

    public int Number
    {
        get; set;
    }

    public string Time
    {
        get; set;
    }

    public string CashierName
    {
        get; set;
    }

    public string Amount
    {
        get; set;
    }

    public string Composition
    {
        get; set;
    }

    public string ExternalNumber
    {
        get; set;
    }

    public string ReceiptNumber
    {
        get; set;
    }

    public void ModelChanged(Change<OrderDto> change)
    {
        var order = change.Current;
        Order = order;

        Debug.Assert(_shiftService.CurrentShift.Value != null);

        Number = order.Id.GuidToPrettyInt();
        Time = order.CreatedAt.ToString("dd.MM.yyyy | HH:mm");
        CashierName = _shiftService.CurrentShift.Value.Member.Name;
        Amount = order.Products.Sum(x => x.TotalPrice + x.Additives.Sum(x => x.TotalPrice)).ToString("F2");
        Composition = string.Join(", ", order.Products.Take(4).Select(x =>
        {
            var productDto = _productService.RuntimeProducts.TryGetValue(x.ProductId, out var product) ? product : null;


            if (productDto is null)
            {
                this.Log().Error($"Product with id {x.ProductId} not found");
            }

            return productDto?.Name ?? "Неизвестный продукт";
        }));

        ExternalNumber = "???";
        ReceiptNumber = "???";
    }

    public void Dispose() {}
}
