using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
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

    public static async ValueTask<bool> TryAuthorizePageAccess<T>(IShiftService shiftservice) where T : PageViewModel
    {
        var mainViewModel = Locator.GetRequiredService<MainViewModel>();
        var pageType = typeof(T);

        // Не впускать работника в сервис
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

        return true;
    }
}
