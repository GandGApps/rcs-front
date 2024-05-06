using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Riok.Mapperly.Abstractions;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.HttpRepository.Api;

[Mapper]
internal static partial class ApiMapper
{
    public static partial OrderDetails MapOrderToRequest(Order order);
    public static partial Order MapRequestToOrder(OrderDetails orderDetails);

    public static OrderDetails MapFullOrderToRequest(Order order)
    {
        var orderDetails = MapOrderToRequest(order);

        orderDetails.PayInfBankCart = order.PaymentInfo!.BankСard;
        orderDetails.PayInfCash = order.PaymentInfo.Cash;
        orderDetails.PayInfCashless = order.PaymentInfo.CashlessPayment;
        orderDetails.PayInfWithoutRev = order.PaymentInfo.WithoutRevenue;
        orderDetails.PayInfChange = order.PaymentInfo.Change;
        orderDetails.PayInfToDeposit = order.PaymentInfo.ToEntered;
        orderDetails.PayInfWithSalesReceipt = order.PaymentInfo.WithSalesReceipt;
        orderDetails.PayInfToDeposit = order.PaymentInfo.ToDeposit;

        return orderDetails;
    }

    public static Order MapRequestToFullOrder(OrderDetails orderDetails)
    {

        var order = MapRequestToOrder(orderDetails);

        order.PaymentInfo = new PaymentInfo
        {
            Id = Guid.NewGuid(),
            BankСard = orderDetails.PayInfBankCart,
            Cash = orderDetails.PayInfCash,
            CashlessPayment = orderDetails.PayInfCashless,
            WithoutRevenue = orderDetails.PayInfWithoutRev,
            Change = orderDetails.PayInfChange,
            ToEntered = orderDetails.PayInfToDeposit,
            WithSalesReceipt = orderDetails.PayInfWithSalesReceipt,
            ToDeposit = orderDetails.PayInfToDeposit
        };

        return order;
    }
}
