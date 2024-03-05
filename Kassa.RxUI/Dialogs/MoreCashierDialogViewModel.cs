using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class MoreCashierDialogViewModel: DialogViewModel
{
    public MoreCashierDialogViewModel(OrderEditPageVm cashierVm) :base()
    {
        AddCommentCommand = cashierVm.CreateCommentCommand;
    }

    public ReactiveCommand<Unit, Unit> AddCommentCommand
    {
        get;
    }
}
