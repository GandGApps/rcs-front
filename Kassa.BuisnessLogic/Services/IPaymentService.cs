using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
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

    public IOrderEditService Order
    {
        get;
    }

    /// <summary>
    /// Pay and save order then dispose
    /// </summary>
    /// <remarks>
    /// Attention! This method also dispose <see cref="IOrderEditService"/> 
    /// </remarks>
    public Task PayAndSaveOrderThenDispose(ReceiptBehavior receiptBehavior);
    public Task PrintReceiptToEmail();
    public Task SetEmailToReceiptSending(string email);
    public Task PayWithBankCard(double money);

}
