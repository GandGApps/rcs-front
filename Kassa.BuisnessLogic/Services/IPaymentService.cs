using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// This service is designed exclusively for processing payments related to a specific order.
/// It is created through the <see cref="ICashierService.CreatePayment(OrderEditDto)"/> method.
/// For handling fund transfers, please use the <see cref="IFundsService"/>.
/// </summary>
public interface IPaymentService : IInitializableService
{
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
        get;
    }

    public double ToEntered
    {
        get;
    }

    public double Change
    {
        get;
    }

    public bool WithSalesReceipt
    {
        get; set;
    }

    public OrderEditDto OrderEditDto
    {
        get;
    }

    /// <summary>
    /// Pay and save order then dispose
    /// </summary>
    public Task PayAndSaveOrderThenDispose(ReceiptBehavior receiptBehavior);
    public Task PrintReceiptToEmail();
    public Task SetEmailToReceiptSending(string email);
    public Task PayWithBankCard(double money);

}
