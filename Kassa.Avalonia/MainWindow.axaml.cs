using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Kassa.Avalonia.Services.MagneticStripeReaders;
using Kassa.RxUI;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using SkiaSharp;
using Splat;

namespace Kassa.Avalonia;
public sealed partial class MainWindow : ReactiveWindow<MainViewModel>
{

    public static readonly AttachedProperty<Visual> PageFooterProperty =
        AvaloniaProperty.RegisterAttached<MainWindow, Control, Visual>("PageFooter");

    public static readonly AttachedProperty<bool> IsHasFooterProperty =
        AvaloniaProperty.RegisterAttached<MainWindow, Control, bool>("IsHasFooter", true);

    public static readonly AttachedProperty<bool> IsGrayscaleEffectOnDialogProperty =
        AvaloniaProperty.RegisterAttached<MainWindow, Control, bool>("IsGrayscaleEffectOnDialog");

    public static void GetIsHasFooter(Control control)
    {
        control.GetValue(IsHasFooterProperty);
    }

    public static void SetIsHasFooter(Control control, bool value)
    {
        control.SetValue(IsHasFooterProperty, value);
    }

    public static void GetIsGrayscaleEffectOnDialog(Control control)
    {
        control.GetValue(IsGrayscaleEffectOnDialogProperty);
    }

    public static void SetIsGrayscaleEffectOnDialog(Control control, bool value)
    {
        control.SetValue(IsGrayscaleEffectOnDialogProperty, value);
    }

    public static void GetPageFooter(Control control)
    {
        control.GetValue(PageFooterProperty);
    }

    public static void SetPageFooter(Control control, Visual value)
    {
        control.SetValue(PageFooterProperty, value);
    }

    public static Control? Root
    {
        get; private set;
    }

    public static MainWindow? Instance
    {
        get; private set;
    } = null!;


    private readonly MsrKeyboardDetector _msrKeyboardDetector = new();

    private readonly BlurEffect _blurEffect = new()
    {
        Radius = 0
    };

    private bool _isGrayscaleEffectOnDialog;

    public MainWindow()
    {
        var viewModel = new MainViewModel();
        DataContext = viewModel;
        ViewModel = viewModel;

        InitializeComponent();

        RootBody.Effect = _blurEffect;

        Root = RootBody;

        TextInput += TryDetectMsr;

        RoutedViewHost.Router = viewModel.Router;
        DialogHost.Router = new();

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
                              if (x is Control element)
                              {
                                  _isGrayscaleEffectOnDialog = element.GetValue(IsGrayscaleEffectOnDialogProperty) is bool isGrayscaleEffectOnDialog && isGrayscaleEffectOnDialog;

                                  if (element.GetValue(IsHasFooterProperty) is bool isHasFooter)
                                  {
                                      Footer.IsVisible = isHasFooter;
                                  }
                                  if (element.GetValue(PageFooterProperty) is Control footer)
                                  {
                                      PageFooterAdditionalContent.Content = footer;
                                      footer.DataContext = viewModel.Router.GetCurrentViewModel();
                                  }
                              }
                          })
                          .DisposeWith(disposables);

            DialogHost.Router.WhenAnyValue(x => x.NavigationStack.Count)
                .Subscribe(x =>
                {
                    if (x == 0)
                    {
                        DialogOverlay.IsVisible = false;
                        _blurEffect.Radius = 0;
                        GrayscaleEffect.IsGrayscale = false;
                    }
                    else
                    {
                        DialogOverlay.IsVisible = true;
                        _blurEffect.Radius = 10;
                        GrayscaleEffect.IsGrayscale = _isGrayscaleEffectOnDialog;
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

        if (GetTopLevel(this) is TopLevel topLevel)
        {
            topLevel.RendererDiagnostics.DebugOverlays |= RendererDebugOverlays.Fps;
        }

        InitFpsCounter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BoundsProperty)
        {
            var width = Bounds.Width;

            BreakpointNotifier.Instance.Width = width;
        }
    }

    private void TryDetectMsr(object? sender, TextInputEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        _msrKeyboardDetector.TryDetect(e.Text);

        LogHost.Default.Debug($"TryDetectMsr: \n\t {e.Text} \n\t RoutingStrategy:{e.Route} \n\t IsHandled:{e.Handled}");
    }
}
