using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Kassa.RxUI;
using SkiaSharp;

namespace Kassa.Avalonia;
public sealed partial class MainWindow : Window
{

    private readonly BlurEffect _blurEffect = new();

    public MainWindow()
    {
        DataContext = new MainViewModel();

        InitializeComponent();

        RootBody.Effect = _blurEffect;
        
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
}
