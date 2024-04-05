using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class ProblemDialogViewModel : DialogViewModel
{

    public ProblemDialogViewModel()
    {

        OkCommand = ReactiveCommand.CreateFromTask(CloseAsync);
    }

    public ReactiveCommand<Unit, Unit> OkCommand
    {
        get;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    [Reactive]
    public string Problem
    {
        get; set;
    } = string.Empty;


    [Reactive]
    public bool IsProblematicDelivery
    {
        get; set;
    }
}
