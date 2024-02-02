using System.Reactive;
using System.Windows.Input;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;

public class MainViewModel : ReactiveObject, IScreen
{
    public RoutingState Router
    {
        get;
    }

    /// <summary>
    /// Subcribe to this command, and implement close logic
    /// TParam, and TResult it's dialog whic called CloseCommand
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

    [Reactive]
    public bool IsMainPage
    {
        get; set;
    }

    public MainViewModel()
    {
        Router = new();
        Router.Navigate.Execute(new MainPageVm(this));

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

        BackToMenuCommand = ReactiveCommand.Create(() =>
        {
            // Remove all from stack except MainPage
            while (Router.NavigationStack.Count > 1)
            {
                Router.NavigationStack.RemoveAt(1);
            }

            IsMainPage = true;
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
