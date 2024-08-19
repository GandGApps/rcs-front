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
using Kassa.RxUI;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;

/// <summary>
/// Interaction logic for WithdrawReasonDialog.xaml
/// </summary>
public partial class WithdrawReasonDialog : SelectionDialogBase<WithdrawReasonDialogViewModel>
{
    public WithdrawReasonDialog()
    {
        InitializeComponent();

        
    }
}
