using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RcsInstaller.Vms;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;
using TruePath;
using Microsoft.Extensions.DependencyInjection;
using RcsInstaller.Services;

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

    public MainWindow(AbsolutePath path): this()
    {
        var mainVm = MainVm.Default;

        

        Task.Run(async () =>
        {
            var appRegistry = App.Host.Services.GetRequiredService<IAppRegistry>();
            var currentVersion = await appRegistry.GetVersion();

            if (currentVersion is null)
            {
                throw new InvalidOperationException("Current version is null");
            }

            var completePageVm = App.CreateInstance<CompletePageVm>(path, currentVersion);

            await mainVm.Router.Navigate.Execute(completePageVm);

            await completePageVm.CheckForUpdates();

        });

        DataContext = mainVm;
    }


    public MainWindow(UpdateOption? updateOption): this()
    {
        var mainVm = MainVm.Default;

        if(updateOption == null)
        {
            mainVm.Router.Navigate.Execute(new WelcomePageVm());
        }
        else
        {
            var installingVm = App.CreateInstance<InstallingPageVm>(updateOption.Path, false, Version.Parse(updateOption.Version));

            mainVm.Router.Navigate.Execute(installingVm);

            installingVm.UpdateCommand.Execute().Subscribe();
        }


        DataContext = mainVm;
    }

}