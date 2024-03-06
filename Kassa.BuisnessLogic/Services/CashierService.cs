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


    public IOrderEditService? CurrentOrder
    {
        get; private set;
    }

    public ReadOnlyObservableCollection<IOrderEditService> Orders
    {
        get;
    }


    public CashierService(IAdditiveService additiveService, ICategoryService categoryService, IProductService productService)
    {
        Orders = new(_orders);
        _additiveService = additiveService;
        _categoryService = categoryService;
        _productService = productService;
    }

    public async ValueTask<IOrderEditService> CreateOrder()
    {
        var order = new OrderEditService(_productService, _categoryService, _additiveService);
        await order.Initialize();

        order.DisposeWith(InternalDisposables);

        return order;

    }

    public async ValueTask<IOrderEditService> CreateOrder(OrderDto order)
    {
        var orderService = new OrderEditService(_productService, _categoryService, _additiveService, order);

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

    public ValueTask<ICashierPaymentService> CreatePayment(IOrderEditService order)
    {
        var paymentService = new CashierPaymentService(order);

        paymentService.Payed += () =>
        {
            _orders.Remove(order);
            if (CurrentOrder == order)
            {
                CurrentOrder = null;
            }
        };

        return new(paymentService);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _categoryService.Initialize();
        await _productService.Initialize();
        await _additiveService.Initialize();
    }
}
