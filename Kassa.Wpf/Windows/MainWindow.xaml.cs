using System.Reactive.Disposables;
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

    public MainWindow()
    {
        Root = RootBody;

        ViewModel = new();

        InitializeComponent();

        RoutedViewHost.Router = ViewModel.Router;

        var viewLocator = Locator.Current.GetService<IViewLocator>() ??
            throw new NullReferenceException($"Service IViewLocator not found!");

        ViewModel.DialogOpenCommand = ReactiveCommand.CreateFromTask(async (DialogViewModel x) =>
        {
            var dialogView = viewLocator.ResolveView(x);
            dialogView!.ViewModel = x;

            if (dialogView is not Control dialogControl)
            {
                return x;
            }

            DialogOverlay.Child = dialogControl;

            ContentBlur.Radius = 10;
            DialogOverlay.Visibility = Visibility.Visible;

            await x.WaitDialogClose();

            ContentBlur.Radius = 0;
            DialogOverlay.Visibility = Visibility.Collapsed;

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

            ViewModel.CloseCommand
                     .Subscribe(x =>
                     {
                         Close();
                     })
                     .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsMainPage, v => v.IsMainPageCheckBox.IsChecked)
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