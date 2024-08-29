using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Kassa.RxUI;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using Kassa.Wpf.Services.MagneticStripeReaders;
using Kassa.Wpf.Windows;
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

    private readonly MsrKeyboardDetector _msrKeyboardDetector = new();

    public static FrameworkElement? Root
    {
        get; private set;
    }

    public static MainWindow? Instance
    {
        get; private set;
    } = null!;

    private readonly KeyConverter _keyConverter = new();

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

            e.Handled = await ViewModel.TryHandleUnhandled(sender, extractedException);
        };

        KeyDown += CopyLogsToClipboard;
        TextInput += TryDetectMsr;

#if SMALL_WINDOW_TEST
        WindowState = WindowState.Normal;
        WindowStyle = WindowStyle.SingleBorderWindow;
        ResizeMode = ResizeMode.CanResize;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
#endif
    }

    private void TryDetectMsr(object sender, TextCompositionEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        _msrKeyboardDetector.TryDetect(e.Text);

        LogHost.Default.Debug($"TryDetectMsr: \n\t {e.Text} \n\t RoutingStrategy:{e.RoutedEvent.RoutingStrategy} \n\t IsHandled:{e.Handled}");
    }

    private void CopyLogsToClipboard(object sender, KeyEventArgs e)
    {

        if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            var stringCollection = GetLogs();

            Clipboard.SetFileDropList(stringCollection);
        }
    }



    private static StringCollection GetLogs()
    {
        var path = App.BasePath;

        var stringCollection = new StringCollection();

        if (Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var fullPath = System.IO.Path.GetFullPath(file);
                stringCollection.Add(fullPath);
            }
        }

        return stringCollection;
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

    private static string FormatException(Exception exception)
    {
        var sb = new StringBuilder();

        sb.AppendLine(exception.Message);
        sb.AppendLine(exception.StackTrace);

        if (exception.InnerException != null)
        {
            sb.AppendLine("Inner exception:");
            sb.AppendLine(FormatException(exception.InnerException));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Try find inner exception of HttpRequestException
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="httpRequestException"></param>
    /// <returns></returns>
    private static bool IsHttpTimeoutException(Exception exception, [NotNullWhen(true)] out HttpRequestException? httpRequestException)
    {
        if (exception is HttpRequestException { HttpRequestError: HttpRequestError.ConnectionError or HttpRequestError.NameResolutionError } requestException)
        {
            httpRequestException = requestException;
            return true;
        }

        if (exception.InnerException != null)
        {
            return IsHttpTimeoutException(exception.InnerException, out httpRequestException);
        }

        httpRequestException = null;
        return false;
    }

}