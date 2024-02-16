using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierPaymentService : IInitializableService, INotifyPropertyChanged
{
    public double Cash
    {
        get; set;
    }

    public double BankСard
    {
        get;
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

    public ICashierService CashierService
    {
        get;
    }

    public Task Pay();
    public Task PrintReceiptToEmail();
    public Task SendReceiptToEmail(string email);
    public Task PayWithBankCard(double money);

}
