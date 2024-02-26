using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierService : IInitializableService
{
    public IOrder? CurrentOrder
    {
        get;
    }

    public ReadOnlyObservableCollection<IOrder> Orders
    {
        get;
    }

    public ValueTask<IOrder> CreateOrder();
    public ValueTask<ICashierPaymentService> CreatePayment(IOrder order);

    public ValueTask SelectCurrentOrder(IOrder order);
}
