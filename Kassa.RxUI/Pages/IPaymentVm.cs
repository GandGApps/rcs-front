﻿using System.Collections.ObjectModel;
using System.Reactive;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public interface IPaymentVm
{
    ReactiveCommand<string, Unit> AddDigitCommand
    {
        get;
    }
    CashierPaymentItemVm BankCardVm
    {
        get;
    }
    ObservableCollection<CashierPaymentItemVm> CashierPaymentItemVms
    {
        get;
    }
    IPaymentService CashierPaymentService
    {
        get;
    }
    CashierPaymentItemVm CashlessPaymentVm
    {
        get;
    }
    CashierPaymentItemVm CashVm
    {
        get;
    }
    double Change
    {
        get;
    }
    string CurrencySymbol
    {
        get;
        set;
    }
    double CurrentPaymentSum
    {
        get;
    }
    string CurrentPaymentSumText
    {
        get;
    }
    PaymentType CurrentPaymentType
    {
        get;
        set;
    }
    string? Email
    {
        get;
        set;
    }
    ReactiveCommand<Unit, Unit> EnableWithCheckboxCommand
    {
        get;
    }
    ReactiveCommand<Unit, Unit> ExactAmountCommand
    {
        get;
    }
    bool IsEmail
    {
        get;
        set;
    }
    bool IsExactAmount
    {
        get;
    }
    bool IsPrinter
    {
        get;
        set;
    }
    ReactiveCommand<double, Unit> PlusCommand
    {
        get;
    }
    string ReceiptActionIcon
    {
        get;
    }
    string ReceiptActionText
    {
        get;
    }
    ReactiveCommand<Unit, Unit> SendReceiptCommand
    {
        get;
    }
    ReactiveCommand<PaymentType, Unit> SetPaymentTypeCommand
    {
        get;
    }
    ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get;
        set;
    }
    double Subtotal
    {
        get;
        set;
    }
    double ToEnter
    {
        get;
    }
    double ToEntered
    {
        get;
    }
    double Total
    {
        get;
        set;
    }
    CashierPaymentItemVm WithoutRevenueVm
    {
        get;
    }
    bool WithReceipt
    {
        get;
        set;
    }
}