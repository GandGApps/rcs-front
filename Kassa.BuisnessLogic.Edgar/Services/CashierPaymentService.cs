using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class CashierPaymentService: BaseInitializableService, IPaymentService
{
    public event Action<OrderDto>? Payed;
    private readonly OrderEditDto _orderEditDto;
    private readonly IOrdersService _ordersService;
    private readonly IPaymentInfoService _paymentInfoService;

    public CashierPaymentService(OrderEditDto orderEditService, IOrdersService ordersService, IPaymentInfoService paymentInfoService)
    {
        _orderEditDto = orderEditService;
        _ordersService = ordersService;
        _paymentInfoService = paymentInfoService;
    }

    public OrderEditDto OrderEditDto => _orderEditDto;

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

    /// <inheritdoc/>
    public async Task PayAndSaveOrderThenDispose(ReceiptBehavior receiptBehavior)
    {
        var order = await _ordersService.CreateOrderAsync(_orderEditDto);

      
        if (receiptBehavior == ReceiptBehavior.PrintReceipt)
        {
            var printer = Splat.Locator.Current.GetRequiredService<IPrinter>();

            await printer.PrintAsync(order);
        }

        if (Cash > 0)
        {
            //var cashDrawer = Splat.Locator.Current.GetRequiredService<ICashDrawer>();

            //await cashDrawer.Open();
        }

        var paymentInfo = new PaymentInfoDto
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Cash = Cash,
            BankСard = BankСard,
            CashlessPayment = CashlessPayment,
            WithoutRevenue = WithoutRevenue,
            ToDeposit = ToDeposit,
            ToEntered = ToEntered,
            WithSalesReceipt = WithSalesReceipt
        };

        order.PaymentInfoId = paymentInfo.Id;
        order.PaymentInfo = paymentInfo;


        await _paymentInfoService.AddPaymentInfo(paymentInfo);

        // It's need for CashierService
        Payed?.Invoke(order);


        Dispose(); // Nothing to dispose, and doesn't need to be called, but it's here for consistency
    }

    public async Task PayWithBankCard(double money)
    {
        // TODO: Implement bank card payment
        await Task.Delay(2000);
        BankСard += money;
    }
    public async Task PrintReceiptToEmail()
    {
        // TODO: Implement email sending
        await Task.Delay(1300);
    }

    public async Task SetEmailToReceiptSending(string email)
    {
        // TODO: Implement email setting
        await Task.Delay(1000);
    }
}
