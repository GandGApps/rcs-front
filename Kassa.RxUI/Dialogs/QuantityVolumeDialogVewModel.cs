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
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public class QuantityVolumeDialogVewModel : DialogViewModel
{
    public QuantityVolumeDialogVewModel(ProductShoppingListItemViewModel productShoppingListItemDto)
    {
        ProductShoppingListItem = productShoppingListItemDto;
        IncorrectPosiblePortionText = productShoppingListItemDto.Count.ToString("0.##", RcsKassa.RuCulture);
        CorrectedPortionText = productShoppingListItemDto.Count.ToString("0.##", RcsKassa.RuCulture);

        AddPortionCommand = ReactiveCommand.Create<double>(x =>
        {
            var count = double.Parse(CorrectedPortionText, RcsKassa.RuCulture);
            count += x;

            CorrectedPortionText = count.ToString("0.##", RcsKassa.RuCulture);
            IncorrectPosiblePortionText = CorrectedPortionText;
        });

        OkCommand = ReactiveCommand.CreateFromTask(async () =>
        { 
            await CloseAsync();

            return double.Parse(CorrectedPortionText, RcsKassa.RuCulture);
        });

        CancelCommand = ReactiveCommand.CreateFromTask(CloseAsync);

        AddPortionCommand.DisposeWith(InternalDisposables);
        OkCommand.DisposeWith(InternalDisposables);
        CancelCommand.DisposeWith(InternalDisposables);
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
                if (double.TryParse(x, RcsKassa.RuCulture, out var correct))
                {
                    CorrectedPortionText = correct.ToString("0.##", RcsKassa.RuCulture);
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

    public ReactiveCommand<Unit, double> OkCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get;
    }

}
