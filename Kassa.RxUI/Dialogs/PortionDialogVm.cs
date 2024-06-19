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

        IntoSeveralEqualParts = new IntoSeveralEqualPartsVm(orderEditService, productShoppingListItemVm).DisposeWith(InternalDisposables);
        IntoTwoUnequalParts = new IntoTwoUnequalPartsVm(orderEditService, productShoppingListItemVm).DisposeWith(InternalDisposables);


        this.WhenAnyValue(x => x.IsIntoSeveralEqualParts)
            .Select<bool, MethodOfDivisionVm>(x => x ? IntoSeveralEqualParts : IntoTwoUnequalParts)
            .ToPropertyEx(this, x => x.CurrentMethodOfDivision)
            .DisposeWith(InternalDisposables);

        IntoSeveralEqualParts.ApplyCommand.Subscribe(_ => CloseCommand.Execute().Subscribe());
        IntoTwoUnequalParts.ApplyCommand.Subscribe(_ => CloseCommand.Execute().Subscribe());

        IsIntoSeveralEqualParts = true;
    }

    public IntoSeveralEqualPartsVm IntoSeveralEqualParts
    {
        get;
    }

    public IntoTwoUnequalPartsVm IntoTwoUnequalParts
    {
        get;
    }

    public extern MethodOfDivisionVm CurrentMethodOfDivision
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    public bool IsIntoSeveralEqualParts
    {
        get; set;
    }

    public abstract class MethodOfDivisionVm : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable _disposables = [];
        protected readonly IOrderEditService _orderEditService;
        protected readonly ProductShoppingListItemViewModel _productShoppingListItemVm;

        public MethodOfDivisionVm(IOrderEditService orderEditService, ProductShoppingListItemViewModel productShoppingListItemVm)
        {
            _orderEditService = orderEditService;
            _productShoppingListItemVm = productShoppingListItemVm;
        }

        [Reactive]
        public double TotalServing
        {
            get; set;
        }

        public abstract ReactiveCommand<Unit, Unit> ApplyCommand
        {
            get;
        }

        public void Dispose() => _disposables.Dispose();
    }

    /// <summary>
    /// CountOfServing / ServingDivider = TotalServing
    /// </summary>
    public sealed class IntoSeveralEqualPartsVm : MethodOfDivisionVm
    {
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

        public override ReactiveCommand<Unit, Unit> ApplyCommand
        {
            get;
        }

        public IntoSeveralEqualPartsVm(IOrderEditService orderEditService, ProductShoppingListItemViewModel productShoppingListItemVm) : base(orderEditService, productShoppingListItemVm)
        {
            // CountOfServing / ServingDivider = TotalServing
            // Когда CountOfServing или ServingDivider меняются, обновляем TotalServing
            this.WhenAnyValue(x => x.CountOfServing, x => x.ServingDivider)
                .Where(x => x.Item2 != 0) // Избегаем деления на ноль
                .Select(x => x.Item1 * x.Item2)
                .BindTo(this, x => x.TotalServing)
                .DisposeWith(_disposables);

            // Когда TotalServing меняется, обновляем ServingDivider
            this.WhenAnyValue(x => x.TotalServing, x => x.CountOfServing)
                .Where(x => x.Item2 != 0) // Избегаем деления на ноль
                .Select(x => x.Item1 / x.Item2)
                .BindTo(this, x => x.ServingDivider)
                .DisposeWith(_disposables);

            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var positionCount = CountOfServing; // Нужно узнать количество продукта в позиции
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
            }).DisposeWith(_disposables);
        }


    }

    public sealed class IntoTwoUnequalPartsVm : MethodOfDivisionVm
    {
        [Reactive]
        public double FirstPart
        {
            get; set;
        }

        [Reactive]
        public double SecondPart
        {
            get; set;
        }

        public override ReactiveCommand<Unit, Unit> ApplyCommand
        {
            get;
        }

        public IntoTwoUnequalPartsVm(IOrderEditService orderEditService, ProductShoppingListItemViewModel productShoppingListItemVm) : base(orderEditService, productShoppingListItemVm)
        {
            // FirstPart + SecondPart = TotalServing

            this.WhenAnyValue(x => x.FirstPart, x => x.SecondPart)
                .Select(x => x.Item1 + x.Item2)
                .BindTo(this, x => x.TotalServing)
                .DisposeWith(_disposables);

            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var source = productShoppingListItemVm.Source;

                var dif = source.Count - FirstPart;

                if (dif > 0)
                {

                    await _orderEditService.DecreaseProductShoppingListItem(source, Math.Abs(FirstPart));
                }
                else if (dif < 0)
                {

                    await _orderEditService.IncreaseProductShoppingListItem(source, Math.Abs(FirstPart));
                }

                var dto = new OrderedProductDto()
                {
                    Id = Guid.NewGuid(),
                    ProductId = source.ItemId,
                    Count = SecondPart,
                    Price = source.Price,
                    Discount = source.Discount,
                };

                await _orderEditService.AddProductToShoppingList(dto);
            });
        }
    }
}
