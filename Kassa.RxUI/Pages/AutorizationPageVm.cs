using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class AutorizationPageVm : PageViewModel
{
    private readonly IAuthService _authService;

    public AutorizationPageVm(IAuthService authService)
    {
        _authService = authService;

        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new AreYouSureToTurnOffDialogViewModel(MainViewModel)).FirstAsync();
        });


        LoginCommand = CreatePageBusyCommand(async () =>
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "Поля не могут быть пустыми";
                return false;
            }

            BusyText = "Авторизация...";

            if (await _authService.AuthenticateAsync(Login, Password))
            {
                return true;
            }

            Error = "Неверный логин или пароль";
            return false;
        });

        Login = string.Empty;
        Password = string.Empty;
        Error = string.Empty;
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
    } 

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
    }

    [Reactive]
    public string Password
    {
        get; set;
    }
}
