using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class PromocodeDialogViewModel : DialogViewModel
{
    public PromocodeDialogViewModel(IOrderEditVm orderEditVm)
    {
        ApplyCommand = ReactiveCommand.CreateFromTask<Unit, IDiscountAccesser?>(async x =>
        {
            if (string.IsNullOrWhiteSpace(Promocode))
            {
                return null;
            }

            var discounts = new Dictionary<Guid, double>
            {
                [Guid.NewGuid()] = 0.30,
                [Guid.NewGuid()] = 0.50
            };

            var discount = IDiscountAccesser.CreateMock(discounts, 0.25);

            await CloseAsync();

            return discount;
        });
    }

    public ReactiveCommand<Unit, IDiscountAccesser?> ApplyCommand
    {
        get;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    [Reactive]
    public string Promocode
    {
        get;
        set;
    } = string.Empty;
}
