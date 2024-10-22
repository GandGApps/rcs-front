using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kassa.Launcher.Vms;
using SukiUI;
using SukiUI.Controls;
using System.Reactive;
using System;
using Avalonia.Controls.ApplicationLifetimes;

namespace Kassa.Launcher;

public partial class MainWindow : SukiWindow
{
    public static MainWindow Instance
    {
        get; private set;
    }

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

        CanResize = false;
        CanMinimize = false;
        BackgroundAnimationEnabled = true;

        Instance = this;

        CanResize = false;
        MinWidth = Width;
        MinHeight = Height;
        MaxWidth = Width;
        MaxHeight = Height;

        var mainVm = MainVm.Default;

        DataContext = mainVm;
    }
}