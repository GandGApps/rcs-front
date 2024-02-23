using System.Reactive.Disposables;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Логика взаимодействия для TurnOffDialogViewModel.xaml
/// </summary>
public partial class TurnOffDialog : ClosableDialog<TurnOffDialogViewModel>
{
    public TurnOffDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm.MainViewModel.CloseCommand, v => v.TurnOffButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton)
                .DisposeWith(disposables);
        });
    }
}
