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
/// Interaction logic for OkMessageDialog.xaml
/// </summary>
public partial class OkMessageDialog : ClosableDialog<OkMessageDialogViewModel>
{
    public OkMessageDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.Icon, view =>  view.Icon.Data, iconName => App.Current.TryFindResource(iconName))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Icon, view => view.Icon.Visibility, iconName => App.Current.TryFindResource(iconName) is null ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Message, view => view.Message.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.OkButtonText, view => view.OkButton.Content)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CloseCommand, view => view.OkButton)
                .DisposeWith(disposables);
        });
    }
}
