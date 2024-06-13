using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Dto;
public sealed class ReportShiftDto
{
    public string Terminal
    {
        get; set;
    }
    public string ShiftNumber
    {
        get; set;
    }
    public DateTime ShiftOpened
    {
        get; set;
    }
    public DateTime CurrentTime
    {
        get; set;
    }
    public string CurrentUser
    {
        get; set;
    }
    public decimal CashSale
    {
        get; set;
    }
    public decimal CashRefund
    {
        get; set;
    }
    public decimal CashPurchase
    {
        get; set;
    }
    public decimal CashPurchaseRefund
    {
        get; set;
    }
    public decimal SberbankSale
    {
        get; set;
    }
    public decimal SberbankRefund
    {
        get; set;
    }
    public decimal TotalSale
    {
        get; set;
    }
    public decimal TotalRefund
    {
        get; set;
    }
    public decimal TotalPurchase
    {
        get; set;
    }
    public decimal TotalPurchaseRefund
    {
        get; set;
    }
    public decimal TotalReplenishment
    {
        get; set;
    }
    public decimal TotalCollection
    {
        get; set;
    }
    public int SaleOperationsCount
    {
        get; set;
    }
    public int RefundOperationsCount
    {
        get; set;
    }
    public int PurchaseOperationsCount
    {
        get; set;
    }
    public int PurchaseRefundOperationsCount
    {
        get; set;
    }
    public int ReplenishmentOperationsCount
    {
        get; set;
    }
    public int CollectionOperationsCount
    {
        get; set;
    }
    public string Notice
    {
        get; set;
    }
}
