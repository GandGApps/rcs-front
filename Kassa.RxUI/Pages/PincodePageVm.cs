using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class PincodePageVm : PageViewModel
{
    [Reactive]
    public string Pincode
    {
        get; set;
    } = null!;

    public string RestoranName
    {
        get;
    }
    public DateTime LicenseEndDate
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CloseCommand
    {
        get;
    }

    public PincodePageVm()
    {
        RestoranName = "RestoranName";
        LicenseEndDate = DateTime.Now;

        CloseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand.Execute(new PincodeTurnOffDialogViewModel()).FirstAsync();
        });
    }

    protected override void OnActivated(CompositeDisposable disposables)
    {
        base.OnActivated(disposables);

        this.WhenAnyValue(x => x.Pincode)
            .Where(x => x is not null && x.Length == 4)
            .Subscribe(async x =>
            {
                Pincode = string.Empty;
                var loading = new LoadingDialogViewModel()
                {
                    Message = "Запрос подключения для модуля: “SUPER_MODUL”"
                };

                MainViewModel.DialogOpenCommand.Execute(loading).Subscribe();
#if RELEASE
                await Task.Delay(10_000);
#elif DEBUG
                await Task.Delay(700);
#endif

                loading.Message = "Типо 10 сек прошло";

                await Task.Delay(1000);

                if (x == "0000")
                {
                    await loading.CloseAsync();
                    await MainViewModel.Router.NavigateAndReset.Execute(new MainPageVm()).FirstAsync();
                    return;
                }

                loading.Message = "Сейчас будет либо 'Неверный пинкод', либо 'Соединение не устоновлено'";

                await Task.Delay(1000);

                await loading.CloseAsync();

                var observableTask = Random.Shared.Next(0, 2) switch
                {
                    0 => MainViewModel.DialogOpenCommand.Execute(new OkMessageDialogViewModel
                    {
                        Icon = "JustFailed",
                        Message = "Неверный пинкод"
                    }).FirstAsync(),
                    1 => MainViewModel.DialogOpenCommand.Execute(new OkMessageDialogViewModel
                    {
                        Icon = "LinkFailed",
                        Message = "Соединение не устоновлено"
                    }).FirstAsync(),
                    _ => throw new InvalidOperationException()
                };

                await observableTask;

            })
            .DisposeWith(disposables);
    }
}
