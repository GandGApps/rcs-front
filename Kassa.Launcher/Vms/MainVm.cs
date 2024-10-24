using Kassa.Launcher.Services;
using Kassa.Launcher.Services;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Launcher.Vms;

public sealed class MainVm : ReactiveObject, IScreen
{
    public static MainVm Default => _mainVm ??= new();

    private static MainVm? _mainVm = null;

    public RoutingState Router { get; }


    private MainVm()
    {
        Router = new RoutingState();

        


        Start = ReactiveCommand.CreateRunInBackground(async void () =>
        {
            var initVm = new LaunchAppVm();

            await Router.Navigate.Execute(initVm).FirstAsync();
        });
        
    }

    public ReactiveCommand<Unit, Unit> Start
    {
        get;
    }
}
