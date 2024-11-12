using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
        IntoTwoUnequalParts = new IntoTwoUnequalPartsVm(orderEditVm, receiptService, productShoppingListItemVm).DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsIntoSeveralEqualParts)
            .Select<bool, MethodOfDivisionVm>(x => x ? IntoSeveralEqualParts : IntoTwoUnequalParts)
            .ToPropertyEx(this, x => x.CurrentMethodOfDivision)
            .DisposeWith(InternalDisposables);

        IntoSeveralEqualParts.ApplyCommand.Subscribe(_ => CloseCommand.Execute().Subscribe());
        IntoTwoUnequalParts.ApplyCommand.Subscribe(_ => CloseCommand.Execute().Subscribe());

        IntoSeveralEqualParts.TotalServing = productShoppingListItemVm.Count;
        IntoSeveralEqualParts.CountOfServing = 1;
        IntoSeveralEqualParts.ServingDivider = productShoppingListItemVm.Count;

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

        protected static bool IsInteger(double value, double eps = 1000) => Math.Abs(value % 1) <= (double.Epsilon * eps);
    }

    /// <summary>
    /// TotalServing = CountOfServing * ServingDivider
    /// </summary>
    public sealed class IntoSeveralEqualPartsVm : MethodOfDivisionVm
    {
        /// <summary>
        /// The number of items in the list
        /// </summary>
        [Reactive]
        public int CountOfServing
        {
            get; set;
        }

        /// <summary>
        /// The amount of product in each item
        /// </summary>
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
            // TotalServing = CountOfServing * ServingDivider

            // Если меняется TotalServing, обновляем CountOfServing
            this.WhenAnyValue(x => x.TotalServing)
                .Select(x => Math.Round(x, 3))
                .Subscribe(x =>
                {
                    if (ServingDivider == 0)
                    {
                        return;
                    }

                    var currentTotal = CountOfServing * ServingDivider;
                    currentTotal = Math.Round(currentTotal, 3);

                    if (currentTotal == x)
                    {
                        TotalServing = x;
                        return;
                    }

                    // Меняем CountOfServing
                    // TotalServing = CountOfServing * ServingDivider
                    // CountOfServing = TotalServing / ServingDivider

                    if (IsInteger(TotalServing / ServingDivider))
                    {
                        CountOfServing = (int)(TotalServing / ServingDivider);
                    }
                    else
                    {
                        // we should change TotalServing
                        // WARNING STACKOVERFLOW 

                        TotalServing = x;
                        ServingDivider = Math.Round(x / CountOfServing, 3);
                    }


                })
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.ServingDivider)
                .Subscribe(x =>
                {
                    var currentTotal = CountOfServing * x;
                    currentTotal = Math.Round(currentTotal, 3);

                    if (currentTotal == Math.Round(TotalServing, 3))
                    {
                        return;
                    }

                    // TotalServing = CountOfServing * ServingDivider
                    // CountOfServing = TotalServing / ServingDivider
                    var countOfServing = Math.Round(TotalServing / x, 3);

                    if (IsInteger(countOfServing))
                    {
                        CountOfServing = (int)countOfServing;
                    }
                    else
                    {
                        // we should change TotalServing
                        // WARNING STACKOVERFLOW 

                        TotalServing = currentTotal;
                    }
                })
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.CountOfServing)
                .Subscribe(x =>
                {
                    if (CountOfServing == 0)
                    {
                        return;
                    }

                    var currentTotal = x * ServingDivider;
                    currentTotal = Math.Round(currentTotal, 3);

                    if (currentTotal == Math.Round(TotalServing, 3))
                    {
                        return;
                    }

                    // TotalServing = CountOfServing * ServingDivider
                    // ServingDivider = TotalServing / CountOfServing
                    ServingDivider = Math.Round(TotalServing / x, 3);
                });


            /*// Когда TotalServing меняется, обновляем ServingDivider
            this.WhenAnyValue(x => x.TotalServing, x => x.CountOfServing)
                .Where(x => x.Item2 != 0) // Избегаем деления на ноль
                .Select(x => Math.Round(x.Item1 / x.Item2, 3))
                .Subscribe(x =>
                {
                    var currentTotal = CountOfServing * x;
                    currentTotal = Math.Round(currentTotal, 3);

                    if (currentTotal == TotalServing)
                    {
                        return;
                    }

                    ServingDivider = Math.Round(x, 3);
                })
                
                //.BindTo(this, x => x.ServingDivider)
                .DisposeWith(_disposables);*/

            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var numberItems = CountOfServing;
                var amountOfProduct = ServingDivider;

                var source = productShoppingListItemVm;

                var dif = source.Count - amountOfProduct;

                if (dif > 0)
                {
                    await _orderEditVm.ShoppingList.DecreaseProductShoppingListItemViewModel(source, Math.Abs(amountOfProduct));
                }
                else if (dif < 0)
                {
                    await _orderEditVm.ShoppingList.IncreaseProductShoppingListItemViewModel(source, Math.Abs(amountOfProduct));
                }

                if (numberItems == 1)
                {
                    return;
                }

                var vm = source.CreateCopyWithoutAdditive();

                var receipt = await _receiptService.GetReceipt(vm.ProductDto.ReceiptId);

                if (receipt is null)
                {
                    this.Log().Error($"Receipt for product {{{vm.ProductDto.Id}}} is null");
                }

                for (var i = 0; i < numberItems - 1; i++)
                {
                    var copyOfVm = vm.CreateCopyUnsafe();

                    // I'm not sure about this line
                    // since I'm not certain if all ingredients are accounted for
                    _orderEditVm.ShoppingList.AddProductShoppingListItemUnsafe(copyOfVm);

                    if (receipt is not null)
                    {
                        // Need to spend ingrdients
                        await _orderEditVm.StorageScope.SpendIngredients(receipt, amountOfProduct);
                    }

                    

                    //await _orderEditVm.AddProductToShoppingList(dto);
                }

                /*var positionCount = CountOfServing; // Нужно узнать количество продукта в позиции
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
                }*/
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

            this.WhenAnyValue(x => x.FirstPart)
                .Subscribe(x =>
                {
                    if(x < 0)
                    {
                        FirstPart = .1;
                        return;
                    }

                    FirstPart = Math.Round(x, 3);
                })
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.SecondPart)
                .Subscribe(x =>
                {
                    if (x < 0)
                    {
                        SecondPart = .1;
                        return;
                    }

                    SecondPart = Math.Round(x, 3);
                })
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.TotalServing)
                .Subscribe(x =>
                {
                    if (x < 0)
                    {
                        TotalServing = .1;
                        return;
                    }

                    var dif = x - (FirstPart + SecondPart);

                    if (dif == 0)
                    {
                        return;
                    }

                    if (dif > 0)
                    {
                        SecondPart = Math.Round(SecondPart + dif, 3);
                    }
                    else
                    {
                        SecondPart = Math.Round(SecondPart + dif, 3);
                    }
                })
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.FirstPart, x => x.SecondPart)
                .Select(x => Math.Round(x.Item1 + x.Item2, 3))
                .BindTo(this, x => x.TotalServing)
                .DisposeWith(_disposables);

            ApplyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // Doen't need to spend ingredients
                // because we are not creating a new product
                // we are just separating the existing one

                var source = productShoppingListItemVm;
                var currentCount = source.Count;

                var dif = currentCount - (FirstPart+SecondPart);

                var receipt = await _receiptService.GetReceipt(source.ProductDto.ReceiptId);

                if (receipt is null)
                {
                    this.Log().Error($"Receipt for product {{{source.ProductDto.Id}}} is null");
                }
                else
                {
                    if (dif > 0)
                    {
                        await _orderEditVm.StorageScope.SpendIngredients(receipt, Math.Abs(FirstPart + SecondPart));
                    }
                    else if (dif < 0)
                    {
                        await _orderEditVm.StorageScope.ReturnIngredients(receipt, Math.Abs(FirstPart + SecondPart));
                    }
                }

                source.Count = FirstPart;

                var vm = source.CreateCopyWithoutAdditive();

                vm.Count = SecondPart;

                _orderEditVm.ShoppingList.AddProductShoppingListItemUnsafe(vm);
            });
        }
    }
}
