﻿using System;
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
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class AutorizationPageVm : PageViewModel
{
    public AutorizationPageVm()
    {
        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new AreYouSureToTurnOffDialogViewModel(MainViewModel)).FirstAsync();
        });


        LoginCommand = CreatePageBusyCommand(async () =>
        {
            var authService = RcsLocator.GetRequiredService<IAuthService>();

            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                Error = "Поля не могут быть пустыми";
                return false;
            }

            BusyText = "Авторизация...";

            if (await authService.AuthenticateAsync(Login, Password))
            {
                return true;
            }

            Error = "Неверный логин или пароль";
            return false;
        });

        Login = string.Empty;
        Password = string.Empty;
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
