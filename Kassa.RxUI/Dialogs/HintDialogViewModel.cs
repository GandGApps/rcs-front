using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class HintDialogViewModel : DialogViewModel
{
    public HintDialogViewModel(IReadOnlyList<HintVm> hintVms)
    {
        Hints = hintVms;

        var firts = Hints[0];

        Target = firts.Target;
        Root = firts.Root;
        Text = firts.Hint;
        Step = 1;


        var canExecuteOkCommand = this.WhenAnyValue(x => x.Step).Select(x => x <= hintVms.Count);

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Step == hintVms.Count)
            {
                await CloseAsync();
                return;
            }
            Step++;

        }, canExecuteOkCommand);
    }

    protected override sealed void OnActivated(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.Step)
                .Subscribe(x =>
                {
                    var hint = Hints[x - 1];

                    Target = hint.Target;
                    Root = hint.Root;
                    Text = hint.Hint;
                })
                .DisposeWith(disposables);
    }

    [Reactive]
    public object Target
    {
        get; private set;
    }

    [Reactive]
    public object? Root
    {
        get; private set;
    }

    [Reactive]
    public string Text
    {
        get; private set;
    }

    [Reactive]
    public int Step
    {
        get; private set;
    }

    public IReadOnlyList<HintVm> Hints
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OkCommand
    {
        get;
    }

}
