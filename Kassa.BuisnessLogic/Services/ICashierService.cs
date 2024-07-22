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
    public OrderEditDto? CurrentOrder
    {
        get;
    }

    public ReadOnlyObservableCollection<OrderEditDto> Orders
    {
        get;
    }


    public ValueTask<OrderEditDto> CreateOrder(bool isDelivery);
    public ValueTask<OrderEditDto> CreateOrder(OrderDto order);
    public ValueTask<IPaymentService> CreatePayment(OrderEditDto order);

    public ValueTask SelectCurrentOrder(OrderEditDto order);
}
