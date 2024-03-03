using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierService : IInitializableService
{
    public IOrderService? CurrentOrder
    {
        get;
    }

    public ReadOnlyObservableCollection<IOrderService> Orders
    {
        get;
    }

    public ValueTask<IOrderService> CreateOrder();
    public ValueTask<ICashierPaymentService> CreatePayment(IOrderService order);

    public ValueTask SelectCurrentOrder(IOrderService order);
}
