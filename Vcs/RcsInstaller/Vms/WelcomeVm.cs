using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;

public sealed class WelcomeVm: PageVm
{
    public WelcomeVm()
    {
        GoNextCommand = ReactiveCommand.Create(() =>
        {
            HostScreen.Router.Navigate.Execute(new SelectPathVm());
        });
    }

    public ReactiveCommand<Unit,Unit> GoNextCommand
    {
        get;
    }

}
