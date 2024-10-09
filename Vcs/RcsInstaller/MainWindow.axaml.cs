using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RcsInstaller.Vms;
using SukiUI.Controls;
using System;

namespace RcsInstaller;

public sealed partial class MainWindow : SukiWindow
{
    public static MainWindow Instance
    {
        get; private set;
    } = null!;

    private MainWindow()
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
    }

    public MainWindow(string path): this()
    {
        var mainVm = MainVm.Default;

        mainVm.Router.Navigate.Execute(new CompleteVm(path));

        DataContext = mainVm;
    }

    public MainWindow(UpdateOption? updateOption): this()
    {
        var mainVm = MainVm.Default;

        if(updateOption == null)
        {
            mainVm.Router.Navigate.Execute(new WelcomeVm());
        }
        else
        {
            var installingVm = new InstallingVm(updateOption.Path, false, Version.Parse(updateOption.Version));

            mainVm.Router.Navigate.Execute(installingVm);

            installingVm.StartInstallCommand.Execute().Subscribe();
        }
        

        DataContext = mainVm;
    }
}