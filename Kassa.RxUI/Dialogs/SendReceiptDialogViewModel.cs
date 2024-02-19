using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class SendReceiptDialogViewModel : DialogViewModel
{
    private readonly CashierPaymentPageVm _cashierPaymentPageVm;
    public SendReceiptDialogViewModel(MainViewModel mainViewModel, CashierPaymentPageVm cashierPaymentPageVm) : base(mainViewModel)
    {
        _cashierPaymentPageVm = cashierPaymentPageVm;
    }

    [Reactive]
    public bool IsSendEmail
    {
        get; set;
    }

    [Reactive]
    public bool IsPrint
    {
        get; set;
    }
}
