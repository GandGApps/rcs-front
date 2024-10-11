using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class OkMessageDialogViewModel : DialogViewModel
{
    [Reactive]
    public string Message
    {
        get; set;
    } = null!;

    [Reactive]
    public string OkButtonText
    {
        get; set;
    } = "OK";

    [Reactive]
    public string Description
    {
        get; set;
    } = string.Empty;

    [Reactive]
    public string Icon
    {
        get; set;
    } = string.Empty;
}
