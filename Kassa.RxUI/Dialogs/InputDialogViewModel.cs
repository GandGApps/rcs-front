using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Kassa.RxUI.Dialogs;
public class InputDialogViewModel : DialogViewModel
{

    public InputDialogViewModel(string fieldName, string? initialValue = null)
    {
        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await CloseAsync();

            return Input;
        }).DisposeWith(InternalDisposables);

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync).DisposeWith(InternalDisposables);

        FieldName = fieldName;

        Input = initialValue;
    }

    [Reactive]
    public string? Input
    {
        get; set;
    }

    public ReactiveCommand<Unit, string?> OkCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

    public string FieldName
    {
        get;
    }
}
