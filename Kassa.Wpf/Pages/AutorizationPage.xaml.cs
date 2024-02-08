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
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для AutorizationPage.xaml
/// </summary>
public partial class AutorizationPage : ReactiveUserControl<AutorizationPageVm>
{
    public static readonly RoutedEvent NextClickedEvent = EventManager.RegisterRoutedEvent("NextClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutorizationPage));

    public event RoutedEventHandler NextClicked
    {

        add => AddHandler(NextClickedEvent, value);
        remove => RemoveHandler(NextClickedEvent, value);
    }

    public AutorizationPage()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.BindCommand(ViewModel, x => x.CloseCommand, x => x.CloseButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Login, x => x.Login.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Password, x => x.Password.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x=> x.LoginCommand, x => x.Submit)
                .DisposeWith(disposables);
        });
    }

    private void LogoSizeAnimationCompleted(object sender, EventArgs e)
    {
        Form.IsHitTestVisible = true;
    }
}
