using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Splat;

namespace Kassa.BuisnessLogic.Services;
internal class CashierService : BaseInitializableService, ICashierService
{
    private readonly ObservableCollection<IOrderEditService> _orders = [];
    private readonly IAdditiveService _additiveService;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IReceiptService _receiptService;
    private readonly IOrdersService _ordersService;
    private readonly IPaymentInfoService _paymentInfoService;

    public IOrderEditService? CurrentOrder
    {
        get; private set;
    }

    public ReadOnlyObservableCollection<IOrderEditService> Orders
    {
        get;
    }


    public CashierService(
        IAdditiveService additiveService,
        ICategoryService categoryService,
        IProductService productService,
        IReceiptService receiptService,
        IOrdersService ordersService,
        IPaymentInfoService paymentInfoService)
    {
        Orders = new(_orders);
        _additiveService = additiveService;
        _categoryService = categoryService;
        _productService = productService;
        _receiptService = receiptService;
        _ordersService = ordersService;
        _paymentInfoService = paymentInfoService;
    }

    public async ValueTask<IOrderEditService> CreateOrder(bool isDelivery)
    {
        var order = new OrderEditService(_productService, _categoryService, _additiveService, _receiptService)
        {
            IsDelivery = isDelivery
        };

        await order.Initialize();

        order.DisposeWith(InternalDisposables);

        return order;

    }

    public async ValueTask<IOrderEditService> CreateOrder(OrderDto order)
    {
        var orderService = new OrderEditService(_productService, _categoryService, _additiveService, _receiptService, order)
        {
            IsDelivery = order.IsDelivery
        };

        await orderService.Initialize();

        orderService.DisposeWith(InternalDisposables);

        foreach (var product in order.Products)
        {
            await orderService.AddProductToShoppingList(product);
        }

        return orderService;
    }


    public ValueTask SelectCurrentOrder(IOrderEditService order)
    {
        CurrentOrder = order;
        return ValueTask.CompletedTask;
    }

    public async ValueTask<IPaymentService> CreatePayment(IOrderEditService orderEdit)
    {
        var paymentService = new CashierPaymentService(orderEdit, _ordersService);

        var order = await orderEdit.GetOrder();

        if (order.PaymentInfoId != Guid.Empty)
        {
            PaymentInfoDto paymentInfo;

            if (order.PaymentInfo != null)
            {
                paymentInfo = order.PaymentInfo;
            }
            else
            {
                paymentInfo = await _paymentInfoService.GetPaymentInfo(order.PaymentInfoId) 
                    ?? throw new InvalidOperationException($"The PaymentInfo with {order.PaymentInfoId} id not found");
            }

            paymentService.Cash = paymentInfo.Cash;
            paymentService.BankСard = paymentInfo.BankСard;
            paymentService.CashlessPayment = paymentInfo.CashlessPayment;
            paymentService.WithoutRevenue = paymentInfo.WithoutRevenue;
        }

        paymentService.Payed += () =>
        {
            _orders.Remove(orderEdit);
            if (CurrentOrder == orderEdit)
            {
                CurrentOrder = null;
            }
        };

        return paymentService;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _categoryService.Initialize();
        await _productService.Initialize();
        await _additiveService.Initialize();
        await _receiptService.Initialize();
        await _ordersService.Initialize();
    }

    protected async override ValueTask DisposeAsyncCore()
    {
        await _categoryService.DisposeAsync();
        await _productService.DisposeAsync();
        await _additiveService.DisposeAsync();
        await _receiptService.DisposeAsync();
        await _ordersService.DisposeAsync();
    }
}
