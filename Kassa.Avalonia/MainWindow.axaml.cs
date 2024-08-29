using System;
using Avalonia;
using Avalonia.Controls;
using Kassa.RxUI;

namespace Kassa.Avalonia;
public sealed partial class MainWindow : Window
{

    public MainWindow()
    {
        DataContext = new MainViewModel();

        InitializeComponent();

        TextInput += (_, _) =>
        {
            throw new Exception("Test exception");
        };
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