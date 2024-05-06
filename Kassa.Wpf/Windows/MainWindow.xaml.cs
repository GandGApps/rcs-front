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

    public static readonly DependencyProperty IsHasFooterProperty =
        DependencyProperty.RegisterAttached("IsHasFooter", typeof(bool), typeof(MainWindow), new(true));

    public static readonly DependencyProperty IsGrayscaleEffectOnDialogProperty =
        DependencyProperty.RegisterAttached("IsGrayscaleEffectOnDialog", typeof(bool), typeof(MainWindow));

    private bool _isGrayscaleEffectOnDialog = false;

    public static FrameworkElement? Root
    {
        get; private set;
    }

    public static MainWindow? Instance
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

        ViewModel.DialogOpenCommand = ReactiveCommand.CreateFromTask(async (DialogViewModel x) =>
        {
            await x.InitializeAsync();

            DialogHost.Router.Navigate.Execute(x).Subscribe();

            await x.CloseCommand.FirstAsync();

            DialogHost.Router.NavigationStack.Remove(x);

            await x.DisposeAsync();

            return x;
        });

        this.WhenActivated(disposables =>
        {
            RoutedViewHost.WhenAnyValue(x => x.Content)
                          .Subscribe(x =>
                          {
                              _isGrayscaleEffectOnDialog = false;
                              if (x is FrameworkElement element)
                              {
                                  _isGrayscaleEffectOnDialog = element.GetValue(IsGrayscaleEffectOnDialogProperty) is bool isGrayscaleEffectOnDialog && isGrayscaleEffectOnDialog;

                                  if (element.GetValue(IsHasFooterProperty) is bool isHasFooter)
                                  {
                                      Footer.Visibility = isHasFooter ? Visibility.Visible : Visibility.Collapsed;
                                  }
                                  if (element.GetValue(PageFooterProperty) is FrameworkElement footer)
                                  {
                                      PageFooterAdditionalContent.Content = footer;
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
                        GrayEffect.EnableGrayscale = 0;
                    }
                    else
                    {
                        DialogOverlay.Visibility = Visibility.Visible;
                        ContentBlur.Radius = 10;
                        GrayEffect.EnableGrayscale = _isGrayscaleEffectOnDialog ? 1 : 0;
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

        Application.Current.DispatcherUnhandledException += async (sender, e) =>
        {
            Exception extractedException;

            if (e.Exception is UnhandledErrorException unhandled)
            {
                extractedException = unhandled.InnerException!;
            }
            else
            {
                extractedException = e.Exception;
            }
#if RELEASE
            if (extractedException is not NotImplementedException)
            {
                e.Handled = true;
                MessageBox.Show(extractedException.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                e.Handled = true;
                await ViewModel.OkMessage("Функция еще не реализована", "JustFailed");
            }
#endif
        };

#if SMALL_WINDOW_TEST
        WindowState = WindowState.Normal;
        WindowStyle = WindowStyle.SingleBorderWindow;
        ResizeMode = ResizeMode.CanResize;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
#endif
    }

    public static void SetPageFooter(UIElement element, object? value)
    {
        element.SetValue(PageFooterProperty, value);
    }

    public static object? GetPageFooter(UIElement element)
    {
        return element.GetValue(PageFooterProperty);
    }

    public static void SetIsHasFooter(UIElement element, bool value)
    {
        element.SetValue(IsHasFooterProperty, value);
    }

    public static bool GetIsHasFooter(UIElement element)
    {
        return (bool)element.GetValue(IsHasFooterProperty);
    }

    public static void SetIsGrayscaleEffectOnDialog(UIElement element, bool value)
    {
        element.SetValue(IsGrayscaleEffectOnDialogProperty, value);
    }

    public static bool GetIsGrayscaleEffectOnDialog(UIElement element)
    {
        return (bool)element.GetValue(IsGrayscaleEffectOnDialogProperty);
    }

}