using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для MainPage.xaml
/// </summary>
public partial class MainPage : ReactiveUserControl<MainPageVm>
{
    public MainPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, x => x.CloseCommand, x => x.CloseButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenProfileDialog, x => x.Profile)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenDocumnetsDialog, x => x.Documents)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenDeliviryDialog, x => x.Deliviry)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenPersonnelDialog, x => x.Personnel)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.OpenServicesDialog, x => x.Services)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.GoToCashier, x => x.Cashbox)
                .DisposeWith(disposables);


            ViewModel!.MainViewModel.DialogOpenCommand.Execute(
                new HintDialogViewModel(ViewModel.MainViewModel, [
                    new("Labu labu dab", Profile, MainWindow.Root),
                    new("asdasdads", Cashbox, MainWindow.Root),
                    new("asdasd", Services, MainWindow.Root),
                    new("sdfddhjk", Deliviry, MainWindow.Root),
                    new("Я в своем познании настолько преисполнился, что я как будто бы уже сто триллионов миллиардов лет проживаю на триллионах и триллионах таких же планет, как эта Земля, мне этот мир абсолютно nпонятен", Personnel, MainWindow.Root),
                    new("!", Documents, MainWindow.Root),
                ])
            ).Subscribe();
        });
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

    }

    private void ButtonWithIcon_Click(object sender, RoutedEventArgs e)
    {

    }
}
