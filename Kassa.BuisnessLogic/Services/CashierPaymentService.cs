using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.BuisnessLogic.Services;
internal class CashierPaymentService(IOrderEditService orderEditService, IOrdersService ordersService) : BaseInitializableService, IPaymentService
{
    public event Action? Payed;

    public IOrderEditService Order => orderEditService;

    public double Cash
    {
        get; set;
    }

    public double BankСard
    {
        get; set;
    }

    public double CashlessPayment
    {
        get; set;
    }

    public double WithoutRevenue
    {
        get; set;
    }

    public double ToDeposit
    {
        get; private set;
    }

    public double ToEntered
    {
        get; private set;
    }

    public double Change => Math.Max(0, ToEntered - ToDeposit);

    public bool WithSalesReceipt
    {
        get; set;
    }

    [Obsolete("Use PayAndSaveOrder")]
    public async Task Pay()
    {
        Payed?.Invoke();
        await Task.Delay(1000);
    }

    public async Task PayAndSaveOrder()
    {
        var order = Order.GetOrder();
        var paymentInfo = new PaymentInfoDto()
        {
            Id = Guid.Empty,
            OrderId = Order.OrderId,
            CashlessPayment = CashlessPayment,
            Cash = Cash,
            BankСard = BankСard,
            WithoutRevenue = WithoutRevenue
        };

        order.PaymentInfo = paymentInfo;
        order.PaymentInfoId = order.PaymentInfoId;

        await ordersService.AddOrder(order);
    }

    public async Task PayWithBankCard(double money)
    {
        await Task.Delay(2000);
        BankСard += money;
    }
    public async Task PrintReceiptToEmail()
    {
        await Task.Delay(1300);
    }

    public async Task SetEmailToReceiptSending(string email)
    {
        await Task.Delay(1000);
    }
}
