using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;

public sealed class MainViewModel : ReactiveObject, IScreen
{
    public static void RegsiterMainViewModel()
    {
        ServiceLocatorBuilder.AddService<IMainViewModelProvider>(new MainViewModelProvider());
        ServiceLocatorBuilder.AddService<MainViewModel>(() =>
        {
            return RcsLocator.GetRequiredService<IMainViewModelProvider>().MainViewModel;
        });
    }

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

    public ReactiveCommand<PageViewModel, PageViewModel> GoToPageAndResetButNotMainCommand
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

    private readonly List<UnhandledErrorExceptionEvent> _unhandledErrorExceptionhandlers = [];

    public event UnhandledErrorExceptionEvent UnhandledErrorExceptionEvent
    {
        remove => _unhandledErrorExceptionhandlers.Remove(value);
        add => _unhandledErrorExceptionhandlers.Add(value);
    }

    public MainViewModel()
    {
        var mainViewModelProvider = RcsLocator.GetRequiredService<IMainViewModelProvider>();
        mainViewModelProvider.MainViewModel = this;

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

        GoToPageAndResetButNotMainCommand = ReactiveCommand.CreateFromTask(async (PageViewModel pageVm) =>
        {
            while (Router.NavigationStack.Count > 0)
            {
                var currentPage = Router.NavigationStack[^1];

                if (currentPage is MainPageVm)
                {
                    break;
                }

                Router.NavigationStack.Remove(currentPage);

                if (currentPage is PageViewModel pageViewModel)
                {
                    pageVm.Activator.Deactivate();

                    await pageVm.DisposeAsync();
                }
            }

            await GoToPage(pageVm);

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
                Message = msg.Message,
                Description = msg.Description
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

        var authService = RcsLocator.GetRequiredService<IAuthService>();
        var shiftService = RcsLocator.GetRequiredService<IShiftService>();
        var reportShiftService = RcsLocator.GetRequiredService<IReportShiftService>();

        authService.CurrentAuthenticationContext.CombineLatest(shiftService.CurrentShift, reportShiftService.CurrentReportShift, (authContext, shift, report) => (authContext, shift, report))
            .Subscribe(async x =>
            {
                if (x.report is not null)
                {
                    await GoToPageAndReset(new CashierShiftReportPageVm(reportShiftService, x.report));

                    return;
                }


                if (!x.authContext.IsAuthenticated)
                {
                    await GoToPageAndReset(new AutorizationPageVm());
                }
                else if (x.shift is null)
                {

                    await GoToPageAndReset(new PincodePageVm());
                }
                else
                {

                    var loading = ShowLoadingDialog("Загрузка данных");
                    try
                    {
                        await RcsLocator.ActivateScope();
                    }
                    catch (Exception exc)
                    {
                        this.Log().Error(exc, "Error on activate scope");
                        await RcsLocator.DisposeScope();
                        throw;
                    }
                    finally
                    {
                        await loading.CloseAsync();
                    }
                    
                    await GoToPageAndReset(new MainPageVm());
                }
            });

        UnhandledErrorExceptionEvent += DefaultUnhandler;
        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(async ex =>
        {
            TryHandleUnhandled("RxApp.DefaultExceptionHandler", ex);
        });


    }

    private void DefaultUnhandler(object? sender, UnhandledErrorExceptionEventArgs e)
    {
        var extractedException = e.Exception;

        this.Log().Error(extractedException, "Unhandled exception");

        if (extractedException is DeveloperException developerException)
        {
            e.Handled = true;
            OkMessage(developerException.Message, "JustFailed");
            return;
        }

        if (extractedException is InvalidUserOperatationException invalidUserOperatationException)
        {
            e.Handled = true;
            OkMessage(invalidUserOperatationException.Message, invalidUserOperatationException.Description, invalidUserOperatationException.Icon);
            return;
        }

        if (IsHttpTimeoutException(extractedException, out _))
        {
            e.Handled = true;
            OkMessage("Проблема с интернетом", "Повторите попытку позже", "JustFailed");
            return;
        }

#if RELEASE
        if (extractedException is not NotImplementedException)
        {
            e.Handled = true;
            OkMessage("Произошла ошибка", e.Exception.Message, "JustFailed");
            return;
        }
        else
        {
            e.Handled = true;
            OkMessage("Функция еще не реализована", "JustFailed");
            return;
        }
#endif
    }

    public void OkMessage(string message, string icon = "JustOk")
    {
        OkMessageDialogCommand.Execute(new OkMessage
        {
            Icon = icon,
            Message = message
        }).Subscribe();
    }

    public async Task OkMessageAsync(string message, string icon = "JustOk")
    {
        await OkMessageDialogCommand.Execute(new OkMessage
        {
            Icon = icon,
            Message = message
        }).FirstAsync();
    }

    public void OkMessage(string message, string description, string icon = "JustOk")
    {
        OkMessageDialogCommand.Execute(new OkMessage
        {
            Icon = icon,
            Message = message,
            Description = description
        }).Subscribe();
    }

    public async Task OkMessageAsync(string message, string description, string icon = "JustOk")
    {

        await OkMessageDialogCommand.Execute(new OkMessage
        {
            Icon = icon,
            Message = message,
            Description = description
        }).FirstAsync();
    }

    public async Task ShowDialogAsync(DialogViewModel dialog)
    {
        await DialogOpenCommand.Execute(dialog).FirstAsync();
    }

    public async Task ShowDialogAndWaitClose(DialogViewModel dialog)
    {
        await DialogOpenCommand.Execute(dialog).FirstAsync();

        await dialog.WaitDialogClose();
    }

    public LoadingDialogViewModel ShowLoadingDialog(string? text)
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

    public async Task<T> RunTaskWithLoadingDialog<T>(string text, Func<LoadingDialogViewModel, Task<T>> task)
    {
        var dialog = ShowLoadingDialog(text);

        var result = await task(dialog);

        await dialog.CloseAsync();

        return result;
    }

    public async Task<T> RunTaskWithLoadingDialog<T>(string text, Task<T> task)
    {
        var dialog = ShowLoadingDialog(text);

        var result = await task;

        await dialog.CloseAsync();

        return result;
    }

    /// <summary>
    /// Use this method in Presentation Layer to handle unhandled exceptions.
    /// </summary>
    public bool TryHandleUnhandled(object? sender, Exception exception)
    {
        var args = new UnhandledErrorExceptionEventArgs(exception);

        foreach (var handler in _unhandledErrorExceptionhandlers)
        {
            handler(sender, args);

            if (args.Handled)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Try find inner exception of HttpRequestException
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="httpRequestException"></param>
    /// <returns></returns>
    private static bool IsHttpTimeoutException(Exception exception, [NotNullWhen(true)] out HttpRequestException? httpRequestException)
    {
        if (exception is HttpRequestException { HttpRequestError: HttpRequestError.ConnectionError or HttpRequestError.NameResolutionError } requestException)
        {
            httpRequestException = requestException;
            return true;
        }

        if (exception.InnerException != null)
        {
            return IsHttpTimeoutException(exception.InnerException, out httpRequestException);
        }

        httpRequestException = null;
        return false;
    }
}
