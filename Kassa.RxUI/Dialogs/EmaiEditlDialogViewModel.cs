using System.Reactive;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class EmaiEditlDialogViewModel : DialogViewModel
{
    private readonly MainViewModel _MainViewModel;
    private readonly CashierPaymentPageVm _cashierPaymentPageVm;

    public EmaiEditlDialogViewModel(CashierPaymentPageVm cashierPaymentPageVm)
    {
        _MainViewModel = MainViewModel;
        _cashierPaymentPageVm = cashierPaymentPageVm;

        PublishEmailCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                _cashierPaymentPageVm.IsEmail = true;
                _cashierPaymentPageVm.IsPrinter = false;
            }

            _cashierPaymentPageVm.Email = Email;

            await CloseAsync();
        });
    }

    public CashierPaymentPageVm CashierPaymentPageVm => _cashierPaymentPageVm;

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