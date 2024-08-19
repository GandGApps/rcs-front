using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kassa.RxUI.Dialogs;

namespace Kassa.Wpf.Dialogs;

public abstract class WidePrimaryDialogBase<T>: ClosableDialog<T> where T: DialogViewModel
{
    static WidePrimaryDialogBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WidePrimaryDialogBase<>), new FrameworkPropertyMetadata(typeof(WidePrimaryDialogBase<>)));
    }

    public WidePrimaryDialogBase()
    {
        ClearValue(TemplateProperty);
    }
}
