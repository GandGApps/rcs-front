﻿using System;
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
                var loading = MainViewModel.ShowLoadingDialog("Запрос подключения для модуля: “SUPER_MODUL”");
                
                try
                {
#if RELEASE
                await Task.Delay(10_000);
#elif DEBUG
                    await Task.Delay(700);
#endif

                    loading.Message = "Типо 10 сек прошло";

                    await Task.Delay(1000);

                    var shiftService = await Locator.GetInitializedService<IShiftService>();


                    if (await shiftService.EnterPincode(x))
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
                finally
                {
                    Pincode = string.Empty;
                    await loading.CloseAsync();
                }
            })
            .DisposeWith(disposables);
    }
}
