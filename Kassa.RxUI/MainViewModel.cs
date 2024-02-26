using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

    public ReactiveCommand<string, Unit> OkMessageDialogCommand
    {
        get; 
    }

    public MainViewModel()
    {
        Router = new();
        Router.Navigate.Execute(new AutorizationPageVm(this));

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

        OkMessageDialogCommand = ReactiveCommand.CreateFromTask<string>(async msg =>
        {
            var okMessageDialog = new OkMessageDialogViewModel
            {
                Icon = "JustFailed",
                Message = msg
            };
            await DialogOpenCommand.Execute().FirstAsync();

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
    }
}
