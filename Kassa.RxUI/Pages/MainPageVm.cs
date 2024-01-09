using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.RxUI.Pages;
public class MainPageVm : PageViewModel
{
    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenProfileDialog
    {
        get;
    }

    public ReactiveCommand<Unit,Unit> OpenDocumnetsDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenDeliviryDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenPersonnelDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenServicesDialog
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OpenSearchDialog
    {
        get;
    }

    public ICommand GoToCashier
    {
        get;
    }

    public MainPageVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new TurnOffDialogViewModel(mainViewModel)).FirstAsync();
        });

        OpenProfileDialog = CreateOpenDialogCommand(() => new ProfileDialogViewModel(mainViewModel));
        OpenDocumnetsDialog = CreateOpenDialogCommand(() => new DocumentsDialogViewModel(mainViewModel));
        OpenDeliviryDialog = CreateOpenDialogCommand(() => new DeliviryDialogViewModel(mainViewModel));
        OpenPersonnelDialog = CreateOpenDialogCommand(() => new PersonnelDialogViewModel(mainViewModel));
        OpenServicesDialog = CreateOpenDialogCommand(() => new ServicesDialogViewModel(mainViewModel));
        OpenSearchDialog = CreateOpenDialogCommand(() => new SearchAddictiveDialogViewModel(mainViewModel));

        GoToCashier = ReactiveCommand.Create(() =>
        {
            MainViewModel.Router.Navigate.Execute(new CashierVm(mainViewModel));
        });
    }

    private ReactiveCommand<Unit,Unit> CreateOpenDialogCommand(Func<DialogViewModel> dialogViewModel) => ReactiveCommand.CreateFromTask(async () =>
    {
        await MainViewModel.DialogOpenCommand.Execute(dialogViewModel()).FirstAsync();
    });
}
