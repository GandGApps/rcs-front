﻿using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;

public class MainViewModel : ReactiveObject, IScreen
{
    /// <summary>
    /// Don't use directly for routing, use <see cref="GoToPageCommand"/> instead.
    /// </summary>
    /// <remarks>
    /// It's public for binding to the view, but don't use it for routing.
    /// </remarks>
    public RoutingState Router
    {
        get;
    }

    /// <summary>
    /// Subcribe to this command, and implement close logic
    /// TParam, and TResult it's dialog which called CloseCommand
    /// </summary>
    public ReactiveCommand<ReactiveObject, ReactiveObject> CloseCommand
    {
        get;
    }

    [Reactive]
    public ReactiveCommand<DialogViewModel, DialogViewModel> DialogOpenCommand
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> BackToMenuCommand
    {
        get;
    }

    public ReactiveCommand<PageViewModel, PageViewModel> GoToPageCommand
    {
        get;
    }

    public ReactiveCommand<PageViewModel, PageViewModel> GoToPageAndResetCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> GoBackCommand
    {
        get;
    }

    [Reactive]
    public bool IsMainPage
    {
        get; set;
    }

    public ReactiveCommand<OkMessage, Unit> OkMessageDialogCommand
    {
        get;
    }

    public MainViewModel()
    {
        Locator.CurrentMutable.RegisterConstant(this);

        Router = new();
        Router.Navigate.Execute(new AutorizationPageVm());

        CloseCommand = ReactiveCommand.Create((ReactiveObject sender) => sender);

        DialogOpenCommand = ReactiveCommand.CreateFromTask(async (DialogViewModel dialog) =>
        {
            await dialog.InitializeAsync();

            return dialog;
        });

        Router.CurrentViewModel.Subscribe(x =>
        {
            IsMainPage = x is MainPageVm;
        });

        BackToMenuCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // Remove all from stack except MainPage
            while (Router.NavigationStack.Count > 1)
            {
                var currentPage = Router.NavigationStack[^1];

                Router.NavigationStack.Remove(currentPage);

                if (currentPage is PageViewModel pageVm)
                {
                    pageVm.Activator.Deactivate();

                    await pageVm.DisposeAsync();
                }
            }

            IsMainPage = true;
        });

        GoToPageCommand = ReactiveCommand.CreateFromTask(async (PageViewModel pageVm) =>
        {
            await pageVm.InitializeAsync();

            await Router.Navigate.Execute(pageVm).FirstAsync();

            return pageVm;
        });

        GoToPageAndResetCommand = ReactiveCommand.CreateFromTask(async (PageViewModel pageVm) =>
        {

            foreach (var vm in Router.NavigationStack)
            {
                if (vm is PageViewModel page)
                {
                    await page.DisposeAsync();
                }
            }

            await pageVm.InitializeAsync();

            Router.NavigationStack.Clear();

            await Router.Navigate.Execute(pageVm).FirstAsync();

            return pageVm;
        });

        GoBackCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Router.NavigationStack.Count > 1)
            {
                var currentPage = Router.NavigationStack[^1];
                Router.NavigationStack.Remove(currentPage);

                if (currentPage is PageViewModel pageVm)
                {
                    pageVm.Activator.Deactivate();

                    await pageVm.DisposeAsync();
                }
            }
        });

        OkMessageDialogCommand = ReactiveCommand.CreateFromTask<OkMessage>(async msg =>
        {
            var okMessageDialog = new OkMessageDialogViewModel
            {
                Icon = msg.Icon,
                Message = msg.Message
            };
            await DialogOpenCommand.Execute(okMessageDialog).FirstAsync();

            await okMessageDialog.WaitDialogClose();
        });

        this.WhenAnyValue(x => x.IsMainPage)
            .Subscribe(x =>
            {
                if (x && Router.GetCurrentViewModel() is not MainPageVm)
                {
                    IsMainPage = true;
                }
            });

        var authService = Locator.Current.GetRequiredService<IAuthService>();
        var shiftService = Locator.Current.GetNotInitializedService<IShiftService>();

        authService.CurrentAuthenticationContext.CombineLatest(shiftService.CurrentShift, (authContext, shift) => (authContext, shift))
            .Subscribe(async x =>
            {

                if (x.authContext is null || !x.authContext.IsAuthenticated)
                {
                    await GoToPageAndReset(new AutorizationPageVm());
                }
                else if (x.shift is null)
                {

                    await GoToPageAndReset(new PincodePageVm());
                }
                else
                {
                    await GoToPageAndReset(new MainPageVm());
                }
            });

    }

    public async Task OkMessage(string message, string icon = "JustOk")
    {
        await OkMessageDialogCommand.Execute(new OkMessage
        {
            Icon = icon,
            Message = message
        }).FirstAsync();
    }

    public async Task ShowDialog(DialogViewModel dialog)
    {
        await DialogOpenCommand.Execute(dialog).FirstAsync();
    }

    public async Task ShowDialogAndWaitClose(DialogViewModel dialog)
    {
        await DialogOpenCommand.Execute(dialog).FirstAsync();

        await dialog.WaitDialogClose();
    }

    public LoadingDialogViewModel ShowLoadingDialog(string text)
    {
        var loadingDialog = new LoadingDialogViewModel
        {
            Message = text
        };

        DialogOpenCommand.Execute(loadingDialog).Subscribe();

        return loadingDialog;
    }

    public async Task GoToPage(PageViewModel pageVm)
    {
        await GoToPageCommand.Execute(pageVm).FirstAsync();
    }

    public async Task GoToPageAndReset(PageViewModel pageVm)
    {
        await GoToPageAndResetCommand.Execute(pageVm).FirstAsync();
    }
}
