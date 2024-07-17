using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;

/// <summary>
/// Логика взаимодействия для SendReceiptDialog.xaml
/// </summary>
public partial class SendReceiptDialog : ClosableDialog<SendReceiptDialogViewModel>
{
    public SendReceiptDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel is null)
            {
                throw new NullReferenceException(nameof(ViewModel));
            }

            if (!(ViewModel.PaymentVm.IsPrinter || ViewModel.PaymentVm.IsEmail))
            {
                ViewModel.PaymentVm.IsEmail = false;
            }

            this.Bind(ViewModel, x => x.PaymentVm.IsPrinter, x => x.Print.IsChecked)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.PaymentVm.IsEmail, x => x.SendEmail.IsChecked)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.PaymentVm.Email, x => x.Email.Text)
                .DisposeWith(disposables);

            this.OneWayBind(
                    ViewModel,
                    x => x.PaymentVm.Email,
                    x => x.Email.Visibility,
                    (email) => string.IsNullOrWhiteSpace(email) ? Visibility.Collapsed : Visibility.Visible
                ).DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditEmailCommand, x => x.EditEmailButton)
                .DisposeWith(disposables);
        });
    }
}
