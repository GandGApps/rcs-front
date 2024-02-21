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

    public SendReceiptDialogViewModel(MainViewModel mainViewModel, CashierPaymentPageVm cashierPaymentPageVm) : base(mainViewModel)
    {
        _cashierPaymentPageVm = cashierPaymentPageVm;

        EditEmailCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.DialogOpenCommand
                    .Execute(new EmaiEditlDialogViewModel(mainViewModel, cashierPaymentPageVm))
                    .FirstAsync();
        });
    }

    public CashierPaymentPageVm CashierPaymentPageVm => _cashierPaymentPageVm;

    public ReactiveCommand<Unit, Unit> EditEmailCommand
    {
        get;
    }
}
