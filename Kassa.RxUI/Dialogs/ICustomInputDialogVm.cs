﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
[Obsolete("It's will be used only for input")]
public interface ICustomInputDialogVm
{
    public ReactiveCommand<Unit, object> OkCommand
    {
        get;
    }
}
