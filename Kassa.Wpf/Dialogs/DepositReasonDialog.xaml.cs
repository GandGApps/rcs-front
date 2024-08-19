using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Interaction logic for DepositReasonDialog.xaml
/// </summary>
public sealed partial class DepositReasonDialog : SelectionDialogBase<DepositReasonDialogViewModel>
{
    public DepositReasonDialog()
    {
        InitializeComponent();
    }
}
