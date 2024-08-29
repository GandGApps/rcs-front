using System;
using Avalonia;
using Avalonia.Controls;

namespace Kassa.Avalonia;
public sealed partial class MainWindow : Window
{

    public MainWindow()
    {
        DataContext = new MainWindow();

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