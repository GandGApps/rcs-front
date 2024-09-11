using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Avalonia.Dialogs;
public abstract class ClosableDialog<T> : BaseDialog<T> where T : DialogViewModel
{
    public ClosableDialog()
    {
        this[!TemplateProperty] = new DynamicResourceExtension("ClosableDialogTemplate");
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);


        if (e.NameScope.Find("CloseDialog") is Button closeDialogButton)
        {
            this.WhenActivated(disposables =>
            {
                ViewModel.WhenAnyValue(x => x.CloseCommand)
                         .Subscribe(x => closeDialogButton.Command = x)
                         .DisposeWith(disposables);
            });
        }
    }
}

