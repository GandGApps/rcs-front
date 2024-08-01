using Kassa.Launcher.Services;
using KassaLauncher.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Vms;

public sealed class MainVm : ReactiveObject, IScreen
{
    public static MainVm Default
    {
        get;
    } = new();

    public RoutingState Router { get; }


    private MainVm()
    {
        Router = new RoutingState();

        var initVm = new InitVm(
            Locator.Current.GetService<IUpdater>()!, 
            Locator.Current.GetService<IInstaller>()!,
            Locator.Current.GetService<IApplicationPathManager>()!);

        

        Start = ReactiveCommand.CreateRunInBackground(async void () =>
        {
            Router.Navigate.Execute(initVm).Subscribe();

            await initVm.InitAsync();
        });
        
    }

    public ReactiveCommand<Unit, Unit> Start
    {
        get;
    }
}
