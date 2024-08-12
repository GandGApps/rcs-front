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
/// Interaction logic for MemberSelectDialog.xaml
/// </summary>
public sealed partial class MemberSelectDialog : ClosableDialog<MemberSelectDialogViewModel>
{
    public MemberSelectDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.FilteredItems, v => v.MemberList.ItemsSource)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.CancelButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Header, v => v.Header.Text)
                .DisposeWith(disposables);

            ViewModel.WhenAnyValue(x => x.Icon)
                .Subscribe(x =>
                {
                    if (string.IsNullOrWhiteSpace(x))
                    {
                        return;
                    }
                    HeaderIcon.Data = (Geometry)FindResource(x);
                })
                .DisposeWith(disposables);
        });
    }
}
