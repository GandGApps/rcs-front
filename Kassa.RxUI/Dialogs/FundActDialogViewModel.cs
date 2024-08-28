using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public sealed class FundActDialogViewModel: DialogViewModel
{

    public FundActDialogViewModel(ReactiveCommand<Unit, Unit>? applyCommand = null)
    {
        ApplyCommand = applyCommand ?? ReactiveCommand.Create(() => { });

        this.WhenAnyValue(x => x.Amount)
            .Select(x => x.ToString("0.##", SharedConstants.RuCulture))
            .ToPropertyEx(this, x => x.AmountText)
            .DisposeWith(InternalDisposables);
    }

    [Reactive]
    public double Amount 
    { 
        get; set; 
    }

    [Reactive]
    public string Comment
    {
        get; set;
    }

    [Reactive]
    public object HeaderTemplateKey
    {
        get; set;
    }

    [Reactive]
    public string Member
    {
        get; set;
    }

    [Reactive]
    public string Reason
    {
        get; set;
    }

    [Reactive]
    public string ApplyButtonText
    {
        get; set;
    }

    [Reactive]
    public bool IsRequiredComment
    {
        get; set;
    }

    public extern string AmountText
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> ApplyCommand
    {
        get; set;
    }
}