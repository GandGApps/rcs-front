using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class QuantityVolumeDialogVewModel : DialogViewModel
{
    public static readonly CultureInfo RuCultureInfo = new("ru-RU");

    public QuantityVolumeDialogVewModel(ProductShoppingListItemViewModel productShoppingListItemDto)
    {
        ProductShoppingListItem = productShoppingListItemDto;
        IncorrectPosiblePortionText = productShoppingListItemDto.Count.ToString("0.##", RuCultureInfo);
        CorrectedPortionText = productShoppingListItemDto.Count.ToString("0.##", RuCultureInfo);

        AddPortionCommand = ReactiveCommand.Create<double>(x =>
        {
            var count = double.Parse(CorrectedPortionText, RuCultureInfo);
            count += x;

            CorrectedPortionText = count.ToString("0.##", RuCultureInfo);
            IncorrectPosiblePortionText = CorrectedPortionText;
        });

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            ProductShoppingListItem.Count = double.Parse(CorrectedPortionText, RuCultureInfo);
            await CloseAsync();
        });

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);
    }

    protected override void OnActivated(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.IncorrectPosiblePortionText)
            .Subscribe(x =>
            {
                if (string.IsNullOrWhiteSpace(x))
                {
                    IncorrectPosiblePortionText = "1";
                    return;
                }
                if (double.TryParse(x, RuCultureInfo, out var correct))
                {
                    CorrectedPortionText = correct.ToString("0.##", RuCultureInfo);
                }
                else
                {
                    IncorrectPosiblePortionText = CorrectedPortionText;
                }
            })
            .DisposeWith(disposables);


    }

    public ProductShoppingListItemViewModel ProductShoppingListItem
    {
        get;
    }

    [Reactive]
    public string IncorrectPosiblePortionText
    {
        get; set;
    }

    [Reactive]
    public string CorrectedPortionText
    {
        get; set;
    }

    public ReactiveCommand<double, Unit> AddPortionCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> OkCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

}
