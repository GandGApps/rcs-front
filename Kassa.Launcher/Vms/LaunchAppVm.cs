using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KassaLauncher.Vms;

public sealed class LaunchAppVm : BaseVm
{

    public LaunchAppVm()
    {
        LaunchAppCommand = ReactiveCommand.Create(() =>
        {

        });
    }

    public ReactiveCommand<Unit, Unit> LaunchAppCommand 
    { 
        get; 
    }

}
