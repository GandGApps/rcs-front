using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class AutorizationPageVm : PageViewModel
{
    public AutorizationPageVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.DialogOpenCommand.Execute(new AreYouSureToTurnOffDialogViewModel(mainViewModel)).FirstAsync();
        });

        LoginCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (Login == "admin" && Password == "admin")
            {
                await mainViewModel.GoToPageCommand.Execute(new PincodePageVm(mainViewModel)).FirstAsync();
                return true;
            }
            else
            {
                Error = "Неверный логин или пароль";
                return false;
            }
        });
    }

    [Reactive]
    public bool IsNextClicked
    {
        get; set;
    }

    [Reactive]
    public string Error
    {
        get; set;
    } = "Типа ошибка";

    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public ReactiveCommand<Unit, bool> LoginCommand
    {
        get;
    }

    [Reactive]
    public string Login
    {
        get; set;
    } = "Типа логин";

    [Reactive]
    public string Password
    {
        get; set;
    } = "Типа пароль";
}
