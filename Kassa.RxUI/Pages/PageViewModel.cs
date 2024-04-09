using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public class PageViewModel : BaseViewModel, IRoutableViewModel
{

    public PageViewModel(MainViewModel MainViewModel) : base(MainViewModel)
    {
        HostScreen = MainViewModel;

        GoBackCommand = ReactiveCommand.Create(() =>
        {
            MainViewModel.GoBackCommand.Execute().Subscribe();
        });
    }

    public PageViewModel() : this(Locator.GetRequiredService<MainViewModel>())
    {

    }

    public string? UrlPathSegment
    {
        get;
    }

    public IScreen HostScreen
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoBackCommand
    {
        get;
    }
}
