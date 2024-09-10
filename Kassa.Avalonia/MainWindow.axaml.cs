using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Kassa.Avalonia.Services.MagneticStripeReaders;
using Kassa.RxUI;
using SkiaSharp;
using Splat;

namespace Kassa.Avalonia;
public sealed partial class MainWindow : Window
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

    public MainWindow()
    {
        DataContext = new MainViewModel();

        InitializeComponent();

        RootBody.Effect = _blurEffect;

        Root = RootBody;

        TextInput += TryDetectMsr;
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
