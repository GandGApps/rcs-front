using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Splat;

namespace Kassa.BuisnessLogic.Services;
internal class CashierService : BaseInitializableService, ICashierService
{
    private readonly ObservableCollection<IOrder> _orders = [];
    private readonly IAdditiveService _additiveService;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;


    public IOrder? CurrentOrder
    {
        get; private set;
    }

    public ReadOnlyObservableCollection<IOrder> Orders
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

    public async ValueTask<IOrder> CreateOrder()
    {
        var order = new Order(_productService, _categoryService, _additiveService);
        await order.Initialize();

        order.DisposeWith(InternalDisposables);

        return order;

    }


    public ValueTask SelectCurrentOrder(IOrder order)
    {
        CurrentOrder = order;
        return ValueTask.CompletedTask;
    }

    public ValueTask<ICashierPaymentService> CreatePayment(IOrder order)
    {
        var paymentService = new CashierPaymentService(order);

        return new(paymentService);
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await _categoryService.Initialize();
        await _productService.Initialize();
        await _additiveService.Initialize();
    }
}
