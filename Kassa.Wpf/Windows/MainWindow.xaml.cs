using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
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
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;
using Splat;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ReactiveWindow<MainViewModel>
{

    public static readonly DependencyProperty PageFooterProperty =
        DependencyProperty.RegisterAttached("PageFooter", typeof(object), typeof(MainWindow));

    public static FrameworkElement? Root
    {
        get; private set;
    }

    public static MainWindow Instance
    {
        get; private set;
    } = null!;

    public MainWindow()
    {
        Instance = this;

        Root = RootBody;

        ViewModel = new();

        InitializeComponent();

        RoutedViewHost.Router = ViewModel.Router;
        DialogHost.Router = new();

        var viewLocator = Locator.Current.GetService<IViewLocator>() ??
            throw new NullReferenceException($"Service IViewLocator not found!");

        ViewModel.DialogOpenCommand = ReactiveCommand.Create((DialogViewModel x) =>
        {
            DialogHost.Router.Navigate.Execute(x);
            x.CloseCommand.FirstAsync().Subscribe(_ =>
            {
                DialogHost.Router.NavigationStack.Remove(x);
            });
            return x;
        });

        this.WhenActivated(disposables =>
        {
            RoutedViewHost.WhenAnyValue(x => x.Content)
                          .Subscribe(x =>
                          {
                              if (x is FrameworkElement element)
                              {
                                  if (element.GetValue(PageFooterProperty) is FrameworkElement footer)
                                  {
                                      PageFooter.Content = footer;
                                      footer.DataContext = ViewModel.Router.GetCurrentViewModel();
                                  }


                              }

                          })
                          .DisposeWith(disposables);

            DialogHost.Router.WhenAnyValue(x => x.NavigationStack.Count)
                .Subscribe(x =>
                {
                    if (x == 0)
                    {
                        DialogOverlay.Visibility = Visibility.Collapsed;
                        ContentBlur.Radius = 0;
                    }
                    else
                    {

                        DialogOverlay.Visibility = Visibility.Visible;
                        ContentBlur.Radius = 10;
                    }
                })
                .DisposeWith(disposables);

            ViewModel.CloseCommand
                     .Subscribe(x =>
                     {
                         Close();
                     })
                     .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.IsMainPage, v => v.IsMainPageCheckBox.IsChecked)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.BackToMenuCommand, v => v.IsMainPageCheckBox)
                .DisposeWith(disposables);

        });


    }
    public static void SetPageFooter(UIElement element, object? value)
    {
        element.SetValue(PageFooterProperty, value);
    }

    public static object? GetPageFooter(UIElement element)
    {
        return element.GetValue(PageFooterProperty);
    }

}