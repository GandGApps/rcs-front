using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;

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

    public IObservableOnlyBehaviourSubject<ICashierShift?> CurrentShift
    {
        get;
    }

    public ValueTask<IOrderEditService> CreateOrder(bool isDelivery);
    public ValueTask<IOrderEditService> CreateOrder(OrderDto order);
    public ValueTask<IPaymentService> CreatePayment(IOrderEditService order);

    public ValueTask SelectCurrentOrder(IOrderEditService order);
}
