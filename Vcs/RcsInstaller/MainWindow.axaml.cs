using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RcsInstaller.Vms;
using SukiUI.Controls;
using System;
using System.ComponentModel;
using TruePath;

namespace RcsInstaller;

public sealed partial class MainWindow : SukiWindow
{
    public static MainWindow Instance
    {
        get; private set;
    } = null!;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
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
    }

    public MainWindow(string path): this()
    {
        var mainVm = MainVm.Default;

        mainVm.Router.Navigate.Execute(App.CreateInstance<CompleteVm>(new AbsolutePath(path)));

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
            var installingVm = App.CreateInstance<InstallingVm>(updateOption.Path, false, Version.Parse(updateOption.Version));

            mainVm.Router.Navigate.Execute(installingVm);

            installingVm.UpdateCommand.Execute().Subscribe();
        }
        

        DataContext = mainVm;
    }
}