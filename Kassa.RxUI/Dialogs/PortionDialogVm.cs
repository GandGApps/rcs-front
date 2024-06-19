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

        IntoSeveralEqualParts = new IntoSeveralEqualPartsVm(orderEditService, productShoppingListItemVm);
        IntoTwoUnequalParts = new IntoTwoUnequalPartsVm(orderEditService, productShoppingListItemVm);

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

    public ReactiveCommand<double, Unit> SetDividerCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> ApplyCommand
    {
        get;
    }

    public abstract class MethodOfDivisionVm : ReactiveObject, IDisposable
    {
        protected readonly CompositeDisposable _disposables = [];
        private readonly IOrderEditService _orderEditService;
        private readonly ProductShoppingListItemViewModel _productShoppingListItemVm;

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
                Observable.Return(42).InvokeCommand(ApplyCommand);
            });
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

        }
    }
}
