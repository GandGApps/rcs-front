using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class LoadingDialogViewModel : DialogViewModel
{
    [Reactive]
    public string? Message
    {
        get; set;
    } = null!;

}
