using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;

public class EnterPincodeDialogViewModel : DialogViewModel
{


    public EnterPincodeDialogViewModel()
    {
        CancelCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Result = null;
            await CloseAsync();
        }).DisposeWith(InternalDisposables);

        BackspaceCommand = ReactiveCommand.Create(() =>
        {
            if (Pincode != null && Pincode.Length > 0)
            {
                Pincode = Pincode[..^1];
            }
        }).DisposeWith(InternalDisposables);

        ClearCommand = ReactiveCommand.Create(() =>
        {
            Pincode = string.Empty;
        }).DisposeWith(InternalDisposables);

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync).DisposeWith(InternalDisposables);

    }

    protected override void Initialize(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.Pincode)
            .Where(x => x is not null && x.Length == 4)
            .Subscribe(async x =>
            {
                Result = x;
                await CloseAsync();
            });
    }

    [Reactive]
    public string? Pincode
    {
        get; set;
    }

    [Reactive]
    public string? Result
    {
        get; private set;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> ClearCommand
    {
        get; 
    }

    public ReactiveCommand<Unit, Unit> BackspaceCommand
    {
        get;
    }
}
