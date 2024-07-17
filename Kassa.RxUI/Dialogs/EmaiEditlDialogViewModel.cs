using System.Reactive;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class EmaiEditlDialogViewModel : DialogViewModel
{
    private readonly IPaymentVm _paymentVm;

    public EmaiEditlDialogViewModel(IPaymentVm cashierPaymentPageVm)
    {
        _paymentVm = cashierPaymentPageVm;

        PublishEmailCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                _paymentVm.IsEmail = true;
            }

            _paymentVm.Email = Email;

            await CloseAsync();
        });
    }

    public IPaymentVm PaymentVm => _paymentVm;

    [Reactive]
    public string? Email
    {
        get;
        set;
    }

    public ReactiveCommand<Unit, Unit> PublishEmailCommand
    {
        get;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get;
        set;
    }
}