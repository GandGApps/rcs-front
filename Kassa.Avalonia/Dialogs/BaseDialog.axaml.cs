using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.ReactiveUI;
using Kassa.RxUI.Dialogs;

namespace Kassa.Avalonia.Dialogs;
public abstract class BaseDialog<T> : ReactiveUserControl<T> where T : DialogViewModel
{
    public BaseDialog()
    {
        this[!TemplateProperty] = new DynamicResourceExtension("BaseDialogTemplate");

    }
}

