using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;

public sealed class MainVm: ReactiveObject, IScreen
{
    public static MainVm Default
    {
        get;
    } = new();

    public RoutingState Router { get; }

    private MainVm()
    {
        Router = new();

    }

}
