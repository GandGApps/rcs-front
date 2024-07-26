using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Dialogs;
public sealed class PortionDialogVm : DialogViewModel
{
    // Perhaps I need to remove these fields
    private readonly IOrderEditVm _orderEditVm;
    private readonly ProductShoppingListItemViewModel _productShoppingListItemVm;
    private readonly IReceiptService _receiptService;

    public PortionDialogVm(IOrderEditVm orderEditVm, IReceiptService receiptService, ProductShoppingListItemViewModel productShoppingListItemVm)
    {
        _receiptService = receiptService;
        _orderEditVm = orderEditVm;
        _productShoppingListItemVm = productShoppingListItemVm;

        IntoSeveralEqualParts = new IntoSeveralEqualPartsVm(orderEditVm, receiptService, productShoppingListItemVm).DisposeWith(InternalDisposables);
        IntoTwoUnequalParts = new IntoTwoUnequalPartsVm(orderEditVm,receiptService, productShoppingListItemVm).DisposeWith(InternalDisposables);

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
        protected readonly IOrderEditVm _orderEditVm;
        protected readonly IReceiptService _receiptService;
        protected readonly ProductShoppingListItemViewModel _productShoppingListItemVm;

        public MethodOfDivisionVm(IOrderEditVm orderEditVm, IReceiptService receiptService, ProductShoppingListItemViewModel productShoppingListItemVm)
        {
            _orderEditVm = orderEditVm;
            _productShoppingListItemVm = productShoppingListItemVm;
            _receiptService = receiptService;
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

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose() => _disposables.Dispose();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
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

        public IntoSeveralEqualPartsVm(IOrderEditVm orderEditVm, IReceiptService receiptService, ProductShoppingListItemViewModel productShoppingListItemVm) : base(orderEditVm, receiptService, productShoppingListItemVm)
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

                var source = productShoppingListItemVm;

                var dif = source.Count - positionCount;

                if (dif > 0)
                {
                    await _orderEditVm.ShoppingList.DecreaseProductShoppingListItemViewModel(source, Math.Abs(positionCount));
                }
                else if (dif < 0)
                {
                    await _orderEditVm.ShoppingList.IncreaseProductShoppingListItemViewModel(source, Math.Abs(positionCount));
                }

                if (totalPositions == 1)
                {
                    return;
                }

                var vm = source.CreateCopyWithoutAdditive();

                var receipt = await _receiptService.GetReceipt(vm.ProductDto.ReceiptId);

                if (receipt is null)
                {
#if DEBUG
                    ThrowHelper.ThrowInvalidOperationException("Receipt is null");
#elif RELEASE
                    this.Log().Error("Receipt is null");
                    return; 
#endif
                }

                for (var i = 0; i < totalPositions - 1; i++)
                {
                    var copyOfVm = vm.CreateCopyUnsafe();

                    // I'm not sure about this line
                    // since I'm not certain if all ingredients are accounted for
                    _orderEditVm.ShoppingList.AddProductShoppingListItemUnsafe(copyOfVm);

                    // Need to spend ingrdients
                    await _orderEditVm.StorageScope.SpendIngredients(receipt, positionCount);

                    //await _orderEditVm.AddProductToShoppingList(dto);
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

        public IntoTwoUnequalPartsVm(IOrderEditVm orderEditVm, IReceiptService receiptService, ProductShoppingListItemViewModel productShoppingListItemVm) : base(orderEditVm, receiptService, productShoppingListItemVm)
        {
            // FirstPart + SecondPart = TotalServing

            this.WhenAnyValue(x => x.FirstPart, x => x.SecondPart)
                .Select(x => x.Item1 + x.Item2)
                .BindTo(this, x => x.TotalServing)
                .DisposeWith(_disposables);

            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // Doen't need to spend ingredients
                // because we are not creating a new product
                // we are just separating the existing one

                var source = productShoppingListItemVm;

                var dif = source.Count - FirstPart;

                source.Count = FirstPart;

                var vm = source.CreateCopyWithoutAdditive();

                var receipt = await _receiptService.GetReceipt(vm.ProductDto.ReceiptId);

                if (receipt is null)
                {
#if DEBUG
                    ThrowHelper.ThrowInvalidOperationException("Receipt is null");
#elif RELEASE
                    this.Log().Error("Receipt is null");
                    return; 
#endif
                }

                vm.Count = SecondPart;
            });
        }
    }
}
