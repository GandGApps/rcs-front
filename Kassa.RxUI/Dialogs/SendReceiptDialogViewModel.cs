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
    private readonly IPaymentVm _paymentVm;

    public SendReceiptDialogViewModel(IPaymentVm paymentVm)
    {
        _paymentVm = paymentVm;

        EditEmailCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand
                    .Execute(new EmaiEditlDialogViewModel(paymentVm))
                    .FirstAsync();
        });
    }

    public IPaymentVm PaymentVm => _paymentVm;

    public ReactiveCommand<Unit, Unit> EditEmailCommand
    {
        get;
    }
}
