using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class SendReceiptDialogViewModel : DialogViewModel
{
    private readonly CashierPaymentPageVm _cashierPaymentPageVm;

    public SendReceiptDialogViewModel(CashierPaymentPageVm cashierPaymentPageVm)
    {
        _cashierPaymentPageVm = cashierPaymentPageVm;

        EditEmailCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand
                    .Execute(new EmaiEditlDialogViewModel(cashierPaymentPageVm))
                    .FirstAsync();
        });
    }

    public CashierPaymentPageVm CashierPaymentPageVm => _cashierPaymentPageVm;

    public ReactiveCommand<Unit, Unit> EditEmailCommand
    {
        get;
    }
}
