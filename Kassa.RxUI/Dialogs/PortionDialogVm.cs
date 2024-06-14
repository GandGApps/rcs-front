using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public sealed class PortionDialogVm : DialogViewModel
{
    private readonly IOrderEditService _orderEditService;
    private readonly ProductShoppingListItemViewModel _productShoppingListItemVm;

    public PortionDialogVm(IOrderEditService orderEditService, ProductShoppingListItemViewModel productShoppingListItemVm)
    {
        _orderEditService = orderEditService;
        _productShoppingListItemVm = productShoppingListItemVm;

        CountOfServing = productShoppingListItemVm.Count;
        ServingDivider = 1;
        TotalServing = productShoppingListItemVm.Count;

        SetDividerCommand = ReactiveCommand.Create<double>(x =>
        {
            ServingDivider = x;
        });

        ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var positionCount = CountOfServing ; // Нужно узнать количество продукта в позиции
            var totalPositions = ServingDivider; // Нужно узнать количество позиций в заказе

            var source = productShoppingListItemVm.Source;

            var dif = source.Count - positionCount;

            if (dif > 0)
            {
                await _orderEditService.DecreaseProductShoppingListItem(source, Math.Abs(positionCount));
            }
            else if (dif < 0)
            {
                await _orderEditService.IncreaseProductShoppingListItem(source, Math.Abs(positionCount));
            }

            for (var i = 0; i < totalPositions - 1; i++)
            {
                var dto = new OrderedProductDto()
                {
                    Id = Guid.NewGuid(),
                    ProductId = source.ItemId,
                    Count = positionCount,
                    Price = source.Price,
                    Discount = source.Discount,
                };

                await _orderEditService.AddProductToShoppingList(dto);
            }

            CloseCommand.Execute().Subscribe();
        });
    }


    [Reactive]
    public double CountOfServing
    {
        get; set;
    }

    [Reactive]
    public double ServingDivider
    {
        get; set;
    }

    [Reactive]
    public double TotalServing
    {
        get; set;
    }

    public ReactiveCommand<double, Unit> SetDividerCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> ApplyCommand
    {
        get;
    }

    protected override void Initialize(CompositeDisposable disposables)
    {
        // CountOfServing / ServingDivider = TotalServing
        // Когда CountOfServing или ServingDivider меняются, обновляем TotalServing
        this.WhenAnyValue(x => x.CountOfServing, x => x.ServingDivider)
            .Where(x => x.Item2 != 0) // Избегаем деления на ноль
            .Select(x => x.Item1 * x.Item2)
            .BindTo(this, x => x.TotalServing)
            .DisposeWith(disposables);

        // Когда TotalServing меняется, обновляем ServingDivider
        this.WhenAnyValue(x => x.TotalServing, x => x.CountOfServing)
            .Where(x => x.Item2 != 0) // Избегаем деления на ноль
            .Select(x => x.Item1 / x.Item2)
            .BindTo(this, x => x.ServingDivider)
            .DisposeWith(disposables);
    }
}
