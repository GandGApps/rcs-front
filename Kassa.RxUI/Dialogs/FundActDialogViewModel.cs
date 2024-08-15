using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public sealed class FundActDialogViewModel: DialogViewModel
{

    public FundActDialogViewModel()
    {
        ApplyCommand = ReactiveCommand.Create(() => { });
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

    public ReactiveCommand<Unit, Unit> ApplyCommand
    {
        get;
    }
}