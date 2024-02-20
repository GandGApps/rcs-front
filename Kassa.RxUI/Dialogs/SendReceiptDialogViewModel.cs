using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class SendReceiptDialogViewModel(MainViewModel mainViewModel, CashierPaymentPageVm cashierPaymentPageVm) : DialogViewModel(mainViewModel)
{
    public CashierPaymentPageVm CashierPaymentPageVm => cashierPaymentPageVm;
}
