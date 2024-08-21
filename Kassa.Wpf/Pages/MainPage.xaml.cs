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
using Kassa.Shared;
using Kassa.Shared.Logging;
using Kassa.Shared.ServiceLocator;
using Kassa.Wpf.Windows;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для MainPage.xaml
/// </summary>
public partial class MainPage : ReactiveUserControl<MainPageVm>, IEnableLogger
{
    private const string KassaNewUserKey = "KASSA_NEW_USER";

    private readonly KonamiSequence _konamiSequence = new();

    public MainPage()
    {
        InitializeComponent();



        this.WhenActivated(disposables =>
        {
            WeakEventManager<Window, KeyEventArgs>.AddHandler(Application.Current.MainWindow, "KeyUp", ReactiveUserControl_KeyDown);

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

            var isFirstStart = Environment.GetEnvironmentVariable(KassaNewUserKey, EnvironmentVariableTarget.User);

            if (string.IsNullOrWhiteSpace(isFirstStart) || isFirstStart == "true")
            {
                ViewModel!.MainViewModel.DialogOpenCommand.Execute(
                    new HintDialogViewModel([
                        new("Labu labu dab", Profile, MainWindow.Root),
                        new("asdasdads", Cashbox, MainWindow.Root),
                        new("asdasd", Services, MainWindow.Root),
                        new("sdfddhjk", Deliviry, MainWindow.Root),
                        new("Я в своем познании настолько преисполнился, что я как будто бы уже сто триллионов миллиардов лет проживаю на триллионах и триллионах таких же планет, как эта Земля, мне этот мир абсолютно nпонятен", Personnel, MainWindow.Root),
                        new("!", Documents, MainWindow.Root),
                    ])
                ).Subscribe();

                Environment.SetEnvironmentVariable(KassaNewUserKey, "false", EnvironmentVariableTarget.User);
            }

            this.OneWayBind(ViewModel, x => x.CurrentShiftMemberName, x => x.CurrentShiftMemberName.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.CurrentShiftOpennedDate, x => x.CurrentShiftOpennedDate.Text, x => x is null ? "Смена не открыта" : $"Смена открыта  {x:dd.MM.yyyy  HH:mm}")
                .DisposeWith(disposables);

            Disposable
                .Create(() => WeakEventManager<Window, KeyEventArgs>.RemoveHandler(Application.Current.MainWindow, "KeyUp", ReactiveUserControl_KeyDown))
                .DisposeWith(disposables);
        });
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

    }

    private void ButtonWithIcon_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ReactiveUserControl_KeyDown(object? sender, KeyEventArgs e)
    {

        if (DeveloperWindow.Instance is not null)
        {
            return;
        }

        if (_konamiSequence.IsCompletedBy(e.Key.ToString()))
        {
            var logger = RcsLocator.GetRequiredService<ObservableLogger>();

            var observable = logger.Select(x => x.Message);

            var developerWindow = new DeveloperWindow(observable)
            {
                Owner = Window.GetWindow(this)
            };

            developerWindow.Show();
        }
    }
}
