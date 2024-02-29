using System.Reactive.Disposables;
using Kassa.RxUI;
using Kassa.RxUI.Dialogs;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Логика взаимодействия для QuantityVolumeDialog.xaml
/// </summary>
public partial class QuantityVolumeDialog : ClosableDialog<QuantityVolumeDialogVewModel>
{
    public QuantityVolumeDialog()
    {
        InitializeComponent();

        Numpad.KeyboardInfo = KeyboardInfo.Numpad();
        NumpadAdditivePorc.KeyboardInfo = KeyboardInfo.NumpadAdditivePorc(null!);

        this.WhenActivated(disposables =>
        {
            ProductName.Text = ViewModel!.ProductShoppingListItem.Name;
            NumpadAdditivePorc.KeyboardInfo = KeyboardInfo.NumpadAdditivePorc(ViewModel!.AddPortionCommand);

            OkButton.Command = ViewModel!.OkCommand;
            CancelButton.Command = ViewModel!.CancelCommand;

            this.OneWayBind(ViewModel, x => x.IncorrectPosiblePortionText, x => x.QuantityVolume.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IncorrectPosiblePortionText, x => x.QuantityVolumeProduct.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.IncorrectPosiblePortionText, x => x.Numpad.Text)
                .DisposeWith(disposables);
        });
    }
}
