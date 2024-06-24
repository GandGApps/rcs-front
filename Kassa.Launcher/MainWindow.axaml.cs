using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using KassaLauncher.Vms;
using SukiUI;
using SukiUI.Controls;
using System.Reactive;
using System;
using Avalonia.Controls.ApplicationLifetimes;

namespace KassaLauncher;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

        CanResize = false;
        CanMinimize = false;
        BackgroundAnimationEnabled = true;

        CanResize = false;
        MinWidth = Width;
        MinHeight = Height;
        MaxWidth = Width;
        MaxHeight = Height;

        var mainVm = MainVm.Default;

        DataContext = mainVm;

        mainVm.Start.Execute().Subscribe();

    }
}