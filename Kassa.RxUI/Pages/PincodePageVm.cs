using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

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

        var magneticStripeReader = Locator.GetService<IMagneticStripeReader>();

        if (magneticStripeReader is null)
        {
            return;
        }

        magneticStripeReader.CardData
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async x =>
            {
                this.Log().Debug("CardData received: {data}", x);

                var data = await x.ReadPincode();

                this.Log().Debug("Pincode received: {data}", data);

                if (data.Length > 4)
                {
                    Pincode = data[..4];
                }
                else if(data.Length == 4)
                {
                    Pincode = data;
                }
            })
            .DisposeWith(InternalDisposables);
    }

    protected override void OnActivated(CompositeDisposable disposables)
    {
        base.OnActivated(disposables);

        this.WhenAnyValue(x => x.Pincode)
            .Where(x => x is not null && x.Length == 4)
            .Subscribe(async x =>
            {
                var loading = MainViewModel.ShowLoadingDialog("Запрос подключения для модуля: “SUPER_MODUL”");

                try
                {
                    var shiftService = await Locator.GetInitializedService<IShiftService>();
                    var authService = Locator.GetRequiredService<IAuthService>();

                    if (await authService.EnterPincode(x))
                    {
                        await MainViewModel.ShowDialog(new OkMessageDialogViewModel()
                        {
                            Message = "Подключение успешно"
                        });
                    }
                    else
                    {
                        await MainViewModel.ShowDialog(new OkMessageDialogViewModel()
                        {
                            Message = "Подключение не удалось"
                        });
                    }
                }
                catch (InvalidUserOperatationException e)
                {
                    await MainViewModel.OkMessage(e.Message, e.Description, e.Icon);
                }
                catch (Exception e)
                {
                    await MainViewModel.ShowDialog(new OkMessageDialogViewModel()
                    {
                        Message = e.Message
                    });
                }
                finally
                {
                    Pincode = string.Empty;
                    await loading.CloseAsync();
                }
            })
            .DisposeWith(disposables);
    }
}
