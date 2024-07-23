using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.Shared;
using Splat;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class CashierService : BaseInitializableService, ICashierService
{
    private readonly IAdditiveService _additiveService;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IReceiptService _receiptService;
    private readonly IOrdersService _ordersService;
    private readonly IPaymentInfoService _paymentInfoService;
    private readonly IShiftService _shiftService;
    private readonly ObservableCollection<OrderEditDto> _orders = [];

    public OrderEditDto? CurrentOrder
    {
        get; private set;
    }

    public ReadOnlyObservableCollection<OrderEditDto> Orders
    {
        get;
    }

    public CashierService(
        IAdditiveService additiveService,
        ICategoryService categoryService,
        IProductService productService,
        IReceiptService receiptService,
        IOrdersService ordersService,
        IPaymentInfoService paymentInfoService,
        IShiftService shiftService)
    {
        Orders = new(_orders);
        _additiveService = additiveService;
        _categoryService = categoryService;
        _productService = productService;
        _receiptService = receiptService;
        _ordersService = ordersService;
        _paymentInfoService = paymentInfoService;
        _shiftService = shiftService;
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _categoryService.Initialize();
        await _productService.Initialize();
        await _additiveService.Initialize();
        await _receiptService.Initialize();
        await _ordersService.Initialize();
        await _shiftService.Initialize();
    }

    protected async override ValueTask DisposeAsyncCore()
    {
        await _categoryService.DisposeAsync();
        await _productService.DisposeAsync();
        await _additiveService.DisposeAsync();
        await _receiptService.DisposeAsync();
        await _ordersService.DisposeAsync();

        // ShiftService created before this service, so it will be disposed after this service
    }

    public ValueTask<OrderEditDto> CreateOrder(bool isDelivery)
    {
        var order = new OrderEditDto()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Status = OrderStatus.New,
            IsForHere = !isDelivery,
            ShowPrice = true,
            IsStopList = false,
            IsDelivery = isDelivery,
        };

        _orders.Add(order);

        return new(order);
    }

    public ValueTask<OrderEditDto> CreateOrder(OrderDto order)
    {
        var orderEdit = new OrderEditDto()
        {

            Id = order.Id,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            IsForHere = order.IsForHere,
            IsDelivery = order.IsDelivery,
            DeliveryTime = order.DeliveryTime,
            CourierId = order.CourierId,
            Comment = order.Comment,
        };

        orderEdit.Products.AddRange(order.Products);


        _orders.Add(orderEdit);

        return new(orderEdit);
    }

    public ValueTask<IPaymentService> CreatePayment(OrderEditDto order)
    {
        throw new NotImplementedException();
    }

    public ValueTask SelectCurrentOrder(OrderEditDto order)
    {
        CurrentOrder = order;

        if (_orders.Contains(order))
        {
            _orders.Remove(order);
        }

        return ValueTask.CompletedTask;
    }
}
