using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public class DeliviryDialogViewModel : DialogViewModel
{
    public DeliviryDialogViewModel()
    {
        GoToAllDeliveriesPage = ReactiveCommand.CreateFromTask(async () =>
        {
            var page = new AllDeliveriesPageVm();

            await CloseAsync();

            await MainViewModel.GoToPageCommand.Execute(page).FirstAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> GoToAllDeliveriesPage
    {
        get;
    }
}
