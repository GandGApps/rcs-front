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
        NumpadAdditivePorc.KeyboardInfo = KeyboardInfo.NumpadAdditivePorc();

        this.WhenActivated(disposables =>
        {
            ProductName.Text = ViewModel!.ProductShoppingListItem.Name;


            this.OneWayBind(ViewModel, x => x.ProductShoppingListItem.Count, x => x.QuantityVolume.Text, x => x.ToString("0.##"))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.ProductShoppingListItem.Count, x => x.QuantityVolumeProduct.Text, x => x.ToString("0.##"))
                .DisposeWith(disposables);
        });
    }
}
