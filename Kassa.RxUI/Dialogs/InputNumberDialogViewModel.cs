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
public class InputNumberDialogViewModel : DialogViewModel
{


    public InputNumberDialogViewModel(string fieldName, string? initialValue = null) : this(fieldName, fieldName, initialValue)
    {
    }

    public InputNumberDialogViewModel(string fieldName, string placeholder, string? initialValue = null) : base()
    {
        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            await CloseAsync();

            return Input;
        });
        OkCommand.DisposeWith(InternalDisposables);

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);
        CancelCommand.DisposeWith(InternalDisposables);

        FieldName = fieldName;
        Placeholder = placeholder;

        Input = initialValue;
    }

    [Reactive]
    public string? Input
    {
        get; set;
    }

    [Reactive]
    public string Placeholder
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

    [Reactive]
    public ReactiveCommand<Unit, Unit>? ClearCommand
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit>? BackspaceCommand
    {
        get; set;
    }
}
