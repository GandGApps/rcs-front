using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierService : IInitializableService
{
    public IOrderEditService? CurrentOrder
    {
        get;
    }

    public ReadOnlyObservableCollection<IOrderEditService> Orders
    {
        get;
    }

    public ValueTask<IOrderEditService> CreateOrder();
    public ValueTask<IOrderEditService> CreateOrder(OrderDto order);
    public ValueTask<ICashierPaymentService> CreatePayment(IOrderEditService order);

    public ValueTask SelectCurrentOrder(IOrderEditService order);
}
