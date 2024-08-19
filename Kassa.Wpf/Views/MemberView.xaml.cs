using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ReactiveUI;

namespace Kassa.Wpf.Views;

/// <summary>
/// Interaction logic for MemberView.xaml
/// </summary>
public sealed partial class MemberView : ReactiveUserControl<MemberVm>
{
    public MemberView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel != null);

            this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text)
                .DisposeWith(disposables);

            SetValue(Button.CommandProperty, ViewModel.SelectCommand);
        });
    }
}
