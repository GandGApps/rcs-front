using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
public abstract class ClosableDialog<T>: ReactiveUserControl<T> where T: DialogViewModel
{
    public ClosableDialog()
    {
        SetResourceReference(TemplateProperty, "ClosableDialogTemplate");
        
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("CloseDialog") is Button closeDialogButton)
        {
            this.WhenActivated(disposables =>
            {
                ViewModel.WhenAnyValue(x => x.CloseCommand)
                         .Subscribe(x => closeDialogButton.Command =  x)
                         .DisposeWith(disposables);
            });
        }
    }
}
