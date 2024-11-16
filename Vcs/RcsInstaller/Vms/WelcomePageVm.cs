using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Vms;

public sealed class WelcomePageVm: PageVm
{
    public WelcomePageVm()
    {
        GoNextCommand = ReactiveCommand.Create(() =>
        {
            HostScreen.Router.Navigate.Execute(App.CreateInstance<SelectPathPageVm>());
        });
    }

    public ReactiveCommand<Unit,Unit> GoNextCommand
    {
        get;
    }

}
