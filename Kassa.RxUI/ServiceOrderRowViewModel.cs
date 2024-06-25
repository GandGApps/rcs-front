using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using ReactiveUI;

namespace Kassa.RxUI;
public class ServiceOrderRowViewModel : ReactiveObject, IGuidId, IApplicationModelPresenter<OrderDto>
{
    private readonly IShiftService _shiftService;
    private readonly IProductService _productService;

    public Guid Id
    {
        get;
    }

    public ServiceOrderRowViewModel(OrderDto order, IShiftService shiftService, IProductService productService, ICashierService cashierService)
    {
        _shiftService = shiftService;
        _productService = productService;


        Id = order.Id;
        Number = order.Id.GuidToPrettyInt();
        Time = order.CreatedAt.ToString("dd.MM.yyyy | HH:mm");
        CashierName = shiftService.Run;
        Amount = order.Products.Sum(x => x.TotalPrice + x.Additives.Sum(x => x.TotalPrice)).ToString("F2");
        Composition = string.Join(", ", order.Products.Take(4).Select(x => x.N));
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
}
