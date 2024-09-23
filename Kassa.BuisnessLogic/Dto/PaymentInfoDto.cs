using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.Dto;
public class PaymentInfoDto : IGuidId
{
    public Guid Id
    {
        get; set;
    }

    public Guid OrderId
    {
        get; set;
    }

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
        get; set;
    }

    public double ToEntered
    {
        get; set;
    }

    public double Change
    {
        get; set;
    }

    public bool WithSalesReceipt
    {
        get; set;
    }
}
