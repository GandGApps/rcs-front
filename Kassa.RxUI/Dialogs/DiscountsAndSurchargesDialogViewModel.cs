using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class DiscountsAndSurchargesDialogViewModel : DialogViewModel
{
    /// <summary>
    /// Reuturns discount. 
    /// Returns <see cref="double.NaN"/> if discount is not set.
    /// </summary>
    public ReactiveCommand<Unit, double> AddCouponCommand
    {
        get;
    }

    public DiscountsAndSurchargesDialogViewModel()
    {
        AddCouponCommand = ReactiveCommand.Create(() => double.NaN);
    }
}
