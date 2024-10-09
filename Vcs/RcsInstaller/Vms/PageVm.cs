using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;

public abstract class PageVm : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment { get; }
    public IScreen HostScreen { get; }

    public PageVm(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public PageVm(): this(MainVm.Default)
    { 
    }
}
