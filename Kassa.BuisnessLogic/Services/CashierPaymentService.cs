using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.BuisnessLogic.Services;
public class CashierPaymentService(IOrderEditService cashierService) : BaseInitializableService, IPaymentService, INotifyPropertyChanged
{
    public event Action? Payed;

    public IOrderEditService Order => cashierService;
    [Reactive]
    public double Cash
    {
        get; set;
    }

    [Reactive]
    public double BankСard
    {
        get; private set;
    }

    [Reactive]
    public double CashlessPayment
    {
        get; set;
    }

    [Reactive]
    public double WithoutRevenue
    {
        get; set;
    }

    [Reactive]
    public double ToDeposit
    {
        get; private set;
    }

    [Reactive]
    public double ToEntered
    {
        get; private set;
    }

    [Reactive]
    public double Change
    {
        get; private set;
    }

    [Reactive]
    public double Total
    {
        get; private set;
    }

    [Reactive]
    public double Subtotal
    {
        get; private set;
    }

    [Reactive]
    public double Discount
    {
        get; private set;
    }

    [Reactive]
    public double Surcharge
    {
        get; private set;
    }

    [Reactive]
    public bool WithSalesReceipt
    {
        get; set;
    }

    public async Task Pay()
    {
        Payed?.Invoke();
        await Task.Delay(1000);
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

    public async Task SendReceiptToEmail(string email)
    {
        await Task.Delay(1000);
    }
}
