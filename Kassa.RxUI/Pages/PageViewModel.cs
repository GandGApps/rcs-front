using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

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

    public PageViewModel(): this(RcsLocator.GetRequiredService<MainViewModel>())
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

    [Reactive]
    public string? BusyText
    {
        get; protected set;
    }

    public ReactiveCommand<Unit, Unit> GoBackCommand
    {
        get;
    }

    protected ReactiveCommand<TParam, TResult> CreatePageBusyCommand<TParam, TResult>(Func<TParam, Task<TResult>> execute)
    {
        return ReactiveCommand.CreateFromTask(async (TParam param) =>
        {
            var loading = MainViewModel.ShowLoadingDialog(BusyText);

            try
            {
                var disposable = this.WhenAnyValue(d => d.BusyText).Subscribe(text => loading.Message = text);

                var result = await execute(param);

                disposable.Dispose();

                BusyText = null;

                return result;
            }
            finally
            {
                await loading.CloseAsync();
            }
        });
    }

    protected ReactiveCommand<Unit, TResult> CreatePageBusyCommand<TResult>(Func<Task<TResult>> execute)
    {
        return ReactiveCommand.CreateFromTask(async () =>
        {
            var loading = MainViewModel.ShowLoadingDialog(BusyText);

            try
            {
                var disposable = this.WhenAnyValue(d => d.BusyText).Subscribe(text => loading.Message = text);

                var result = await execute();

                disposable.Dispose();

                BusyText = null;

                return result;
            }
            finally
            {
                await loading.CloseAsync();
            }
        });
    }

    protected ReactiveCommand<Unit, Unit> CreatePageBusyCommand(Func<Task> execute)
    {
        return ReactiveCommand.CreateFromTask(async () =>
        {
            var loading = MainViewModel.ShowLoadingDialog(BusyText);

            try
            {
                var disposable = this.WhenAnyValue(d => d.BusyText).Subscribe(text => loading.Message = text);

                using (disposable)
                {
                    await execute();

                    BusyText = null;
                }
            }
            finally
            {
                await loading.CloseAsync();
            }
        });
    }

    public static async ValueTask<bool> TryAuthorizePageAccess<T>(IShiftService shiftservice) where T : PageViewModel
    {
        var mainViewModel = RcsLocator.GetRequiredService<MainViewModel>();
        var pageType = typeof(T);

        // Не впускать работника в сервис
        if (shiftservice.CurrentShift.Value is IShift shift && !shift.Member.IsManager && pageType == typeof(ServicePageVm))
        {
            await mainViewModel.OkMessage("Этот раздел только для менеджеров", "JustFailed");
            return false;
        }

        // Не впускать никуда кроме сервиса если не начата кассовая смена
        if (!shiftservice.IsCashierShiftStarted() && pageType != typeof(ServicePageVm))
        {
            await mainViewModel.OkMessage("Кассовая смена не открыта", "JustFailed");
            return false;
        }

        // Не впускать во все страницы кроме персонального кабинета если
        // не начата обычная смена
        if (shiftservice.CurrentCashierShift.Value == null && !shiftservice.IsShiftStarted() && pageType != typeof(PersonalPageVm))
        {
            await mainViewModel.OkMessage("Смена не открыта", "JustFailed");
            return false;
        }

        return true;

        /*// Не впускать работника в сервис
        if (shiftservice.CurrentShift.Value != null && pageType == typeof(ServicePageVm))
        {
            await mainViewModel.OkMessage("Этот раздел только для менеджеров", "JustFailed");
            return false;
        }

        // Не впускать никуда кроме сервиса если не начата кассовая смена
        if (!shiftservice.IsCashierShiftStarted() && pageType != typeof(ServicePageVm))
        {
            await mainViewModel.OkMessage("Кассовая смена не открыта", "JustFailed");
            return false;
        }

        // Не впускать менеджера никуда кроме сервиса
        if (shiftservice.CurrentCashierShift.Value != null && shiftservice.CurrentShift.Value == null && pageType != typeof(ServicePageVm))
        {
            await mainViewModel.OkMessage("Этот раздел только для работников", "JustFailed");
            return false;
        }

        // Не впускать во все страницы кроме персонального кабинета если
        // не начата обычная смена
        if (shiftservice.CurrentCashierShift.Value == null && !shiftservice.IsShiftStarted() && pageType != typeof(PersonalPageVm))
        {
            await mainViewModel.OkMessage("Смена не открыта", "JustFailed");
            return false;
        }

        return true;*/
    }
}
