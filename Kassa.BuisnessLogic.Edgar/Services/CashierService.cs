using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using DynamicData;
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

    public ValueTask<OrderEditDto> CreateOrder(bool isDelivery)
    {
        var order = new OrderEditDto()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Status = OrderStatus.New,
            IsPickup = !isDelivery,
            ShowPrice = true,
            IsStopList = false,
            IsDelivery = isDelivery,
        };

        _orders.Add(order);

        return new(order);
    }

    public async ValueTask<OrderEditDto> CreateOrder(OrderDto order)
    {
        var orderEdit = Mapper.MapOrderDtoToOrderEditDto(order);

        if (order.Products is not null)
        {
            foreach (var orderedProduct in order.Products)
            {
                var product = await _productService.GetProductById(orderedProduct.ProductId);

                if (product is null)
                {
#if DEBUG
                    ThrowHelper.ThrowInvalidOperationException($"Product with id={orderedProduct.ProductId} not found");
#elif RELEASE
                    this.Log().Error("Product with id={0} not found", orderedProduct.ProductId);
                    continue;
#endif
                }

                var productShoppingListItem = new ProductShoppingListItemDto(orderedProduct, product);
                orderEdit.Products.Add(productShoppingListItem);
            }
        }


        // We don't need to add order to the list of orders
        // because it is use to read data from the database
        //_orders.Add(orderEdit);

        return orderEdit;
    }

    public ValueTask<IPaymentService> CreatePayment(OrderEditDto orderEdit)
    {
        var paymentService = RcsKassa.CreateAndInject<CashierPaymentService>(orderEdit);

        paymentService.Payed += (order) =>
        {
            var shiftId = _shiftService.CurrentShift.Value!.CreateDto().Id;
            var cashierShiftId = _shiftService.CurrentCashierShift.Value!.CreateDto().Id;

            order.ShiftId = shiftId;
            order.CashierShiftId = cashierShiftId;

            _ordersService.AddOrder(order);
        };

        return ValueTask.FromResult<IPaymentService>(paymentService);
    }

    public ValueTask SelectCurrentOrder(OrderEditDto order)
    {
        CurrentOrder = order;

        _orders.Remove(order);

        return ValueTask.CompletedTask;
    }
}
