using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public abstract class BasePaymentPageVm: PageViewModel, IPaymentVm
{
    protected bool _isFloat;
    protected byte _afterSeparatorDigitCount;

    protected readonly ICashierService _cashierService;
    protected readonly IAdditiveService _additiveService;
    protected readonly IProductService _productService;
    protected readonly IIngridientsService _ingridientsService;
    protected readonly IOrderEditVm _orderEditVm;
    protected readonly IReceiptService _receiptService;
    protected readonly ICategoryService _categoryService;

    public BasePaymentPageVm(
        ReadOnlyObservableCollection<ProductShoppingListItemViewModel> shoppingListItems,
        IOrderEditVm orderEditVm,
        IPaymentService paymentService,
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        IIngridientsService ingridientsService,
        IReceiptService receiptService,
        ICategoryService categoryService)
    {
        PaymentService = paymentService;

        _orderEditVm = orderEditVm;
        _cashierService = cashierService;
        _additiveService = additiveService;
        _productService = productService;
        _ingridientsService = ingridientsService;
        _receiptService = receiptService;
        _categoryService = categoryService;

        ShoppingListItems = shoppingListItems;

        CashVm = new()
        {
            Description = "Наличные",
        };
        BankCardVm = new()
        {
            Description = "Банковская карта",
        };
        CashlessPaymentVm = new()
        {
            Description = "Безналичный расчет",
        };
        WithoutRevenueVm = new()
        {
            Description = "Без выручки",
        };

        CashierPaymentItemVms = [CashVm, BankCardVm, CashlessPaymentVm, WithoutRevenueVm];

        this.WhenAnyValue(x => x.IsEmail, x => !x)
            .ToPropertyEx(this, x => x.IsPrinter)
            .DisposeWith(InternalDisposables);

        SendReceiptCommand = ReactiveCommand.Create(() => { });
        EnableWithCheckboxCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await MainViewModel.DialogOpenCommand
                    .Execute(new SendReceiptDialogViewModel(this))
                    .FirstAsync();
        });

        AddDigitCommand = ReactiveCommand.Create<string>(digit =>
        {
            if (digit == "C")
            {
                ClearCurrentPaymentSum();
                return;
            }

            if (digit == "," && !_isFloat)
            {
                _isFloat = true;
                if (CurrentPaymentSumText.Contains(','))
                {
                    return;
                }
                CurrentPaymentSumText += digit;
                return;
            }

            if (_afterSeparatorDigitCount >= 2)
            {
                return;
            }

            if (!char.IsDigit(digit[0]))
            {
                return;
            }

            if (_isFloat)
            {
                _afterSeparatorDigitCount++;
            }

            CurrentPaymentSumText += digit;

            return;
        });

        PlusCommand = ReactiveCommand.Create<double>((x) =>
        {
            var value = CurrentPaymentSum + x;
            CurrentPaymentSumText = value.ToString($"F{_afterSeparatorDigitCount}");
        });

        SetPaymentTypeCommand = ReactiveCommand.Create<PaymentType>(type =>
        {
            // Don't allow to change payment type if the locker is true
            Locker = true;

            ClearCurrentPaymentSum();

            if (type == PaymentType.Cash)
            {
                SetDisplayText(CashVm.Entered);
            }
            else if (type == PaymentType.BankCard)
            {
                SetDisplayText(BankCardVm.Entered);
            }
            else if (type == PaymentType.CashlessPayment)
            {
                SetDisplayText(CashlessPaymentVm.Entered);
            }
            else if (type == PaymentType.WithoutRevenue)
            {
                SetDisplayText(WithoutRevenueVm.Entered);
            }

            CurrentPaymentType = type;
            Locker = false;
        });

        ExactAmountCommand = ReactiveCommand.Create(() =>
        {
            var exactAmount = Total - ToEntered < 0 ? Total : ToEnter;
            if (CurrentPaymentType == PaymentType.Cash)
            {
                SetDisplayText(exactAmount);
            }
            else if (CurrentPaymentType == PaymentType.BankCard)
            {
                SetDisplayText(exactAmount);
            }
            else if (CurrentPaymentType == PaymentType.CashlessPayment)
            {
                SetDisplayText(exactAmount);
            }
            else if (CurrentPaymentType == PaymentType.WithoutRevenue)
            {
                SetDisplayText(exactAmount);
            }
        });

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, x => x.WithReceipt, (email, printer, withReceipt) =>
        {
            if (!withReceipt)
            {
                return "Распечатать, переслать чек";
            }

            if (email)
            {
                return "Отправить на почту";
            }

            return "Распечатать";
        })
            .ToPropertyEx(this, x => x.ReceiptActionText)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) =>
        {
            if (email)
            {
                return "EmailIcon";
            }
            else if (printer)
            {
                return "PrinterIcon";
            }
            else
            {
                return "PrinterIcon";
            }
        })
            .ToPropertyEx(this, x => x.ReceiptActionIcon)
            .DisposeWith(InternalDisposables);


        CashierPaymentItemVms.ToObservableChangeSet()
            .AutoRefresh(x => x.Entered)
            .ToCollection()
            .Select(x => x.Sum(x => x.Entered))
            .ToPropertyEx(this, x => x.ToEntered)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.ToEntered, x => x.Total, (entered, total) => Math.Max(0, total - entered))
            .ToPropertyEx(this, x => x.ToEnter)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.ToEnter, x => x.ToEntered, x => x.Total, (toEnter, entered, total) => Math.Max(0, entered - total))
            .ToPropertyEx(this, x => x.Change)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.Total, x => x.ToEntered, (total, entered) => total <= entered)
            .ToPropertyEx(this, x => x.IsExactAmount)
            .DisposeWith(InternalDisposables);

        this.WhenAnyValue(x => x.CurrentPaymentSumText, (text) =>
        {
            text = text.Replace(',', '.');
            return string.IsNullOrWhiteSpace(text) ? 0 : text.EndsWith('.') ? double.Parse(text[..^1]) : double.Parse(text);
        })
            .ToPropertyEx(this, x => x.CurrentPaymentSum)
            .DisposeWith(InternalDisposables);

        // TODO: Replace with page busy command
        PayCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var loading = new LoadingDialogViewModel()
            {
                Message = "Оплата..."
            };

            MainViewModel.DialogOpenCommand.Execute(loading).Subscribe();

            var receiptBehavior = ReceiptBehavior.NoPrintReceipt;

            if (WithReceipt)
            {
                if (IsEmail)
                {
                    receiptBehavior |= ReceiptBehavior.SendToEmail;
                }

                if (IsPrinter)
                {
                    receiptBehavior |= ReceiptBehavior.PrintReceipt;
                }
            }

            

            paymentService.Cash = CashVm.Entered;
            paymentService.BankСard = BankCardVm.Entered;
            paymentService.CashlessPayment = CashlessPaymentVm.Entered;
            paymentService.WithoutRevenue = WithoutRevenueVm.Entered;

            await paymentService.PayAndSaveOrderThenDispose(receiptBehavior);

            await loading.CloseAsync();

            if (Change - 0.001 >= 0)
            {
                await MainViewModel.OkMessageAsync("Сдача \n" + Change + " " + CurrencySymbol, "");
            }

            await MainViewModel.OkMessageAsync("Оплата прошла успешно", "");

            var order = await _cashierService.CreateOrder(false);

            await _cashierService.SelectCurrentOrder(order);

            var orderEditPageVm = new OrderEditPageVm(order, _ingridientsService.CreateStorageScope(), _cashierService, _additiveService, _productService, _categoryService, _receiptService, _ingridientsService);

            await MainViewModel.GoToPageAndResetButNotMainCommand.Execute(orderEditPageVm);

        }, this.WhenAnyValue(x => x.IsExactAmount));
    }

    public IPaymentService PaymentService
    {
        get;
    }

    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel> ShoppingListItems
    {
        get; 
    }

    public ReactiveCommand<PaymentType, Unit> SetPaymentTypeCommand
    {
        get;
    }


    [Reactive]
    public double Subtotal
    {
        get; set;
    }

    [Reactive]
    public double Total
    {
        get; set;
    }

    [Reactive]
    public string? Email
    {
        get; set;
    } = string.Empty;

    [Reactive]
    public bool WithReceipt
    {
        get; set;
    }

    [Reactive]
    public PaymentType CurrentPaymentType
    {
        get; set;
    } = PaymentType.Cash;

    [Reactive]
    public bool IsEmail
    {
        get; set;
    }

    public extern bool IsPrinter
    {
        [ObservableAsProperty]
        get; 
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = "₽";


    public extern string ReceiptActionText
    {
        [ObservableAsProperty]
        get;
    }

    public extern string ReceiptActionIcon
    {
        [ObservableAsProperty]
        get;
    }

    public extern double ToEnter
    {
        [ObservableAsProperty]
        get;
    }


    public extern double ToEntered
    {
        [ObservableAsProperty]
        get;
    }

    public extern double Change
    {
        [ObservableAsProperty]
        get;
    }

    public ReactiveCommand<Unit, Unit> SendReceiptCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> EnableWithCheckboxCommand
    {
        get;
    }

    public ObservableCollection<CashierPaymentItemVm> CashierPaymentItemVms
    {
        get;
    } = [];

    public CashierPaymentItemVm CashVm
    {
        get;
    }

    public CashierPaymentItemVm BankCardVm
    {
        get;
    }

    public CashierPaymentItemVm CashlessPaymentVm
    {
        get;
    }

    public CashierPaymentItemVm WithoutRevenueVm
    {
        get;
    }


    public ReactiveCommand<double, Unit> PlusCommand
    {
        get;
    }

    public ReactiveCommand<string, Unit> AddDigitCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> ExactAmountCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> PayCommand
    {
        get;
    }

    [Reactive]
    public string CurrentPaymentSumText
    {
        get; protected set;
    } = string.Empty;

    public extern double CurrentPaymentSum
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    protected bool Locker
    {
        get; set;
    }

    public extern bool IsExactAmount
    {
        [ObservableAsProperty]
        get;
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => Subtotal = x)
                         .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Subtotal)
            .Subscribe(x => Total = x)
            .DisposeWith(disposables);


        this.WhenAnyValue(x => x.CurrentPaymentType, x => x.CurrentPaymentSum, x => x.Locker)
            .Subscribe<(PaymentType type, double sum, bool locker)>(x =>
            {
                if (x.locker)
                {
                    return;
                }

                if (x.type == PaymentType.Cash)
                {
                    CashVm.Entered = x.sum;
                }
                else if (x.type == PaymentType.BankCard)
                {
                    BankCardVm.Entered = x.sum;
                }
                else if (x.type == PaymentType.CashlessPayment)
                {
                    CashlessPaymentVm.Entered = x.sum;
                }
                else if (x.type == PaymentType.WithoutRevenue)
                {
                    WithoutRevenueVm.Entered = x.sum;
                }
            })
            .DisposeWith(disposables);


        CashVm.RemoveItemCommand.Subscribe(_ =>
        {
            if (CurrentPaymentType == PaymentType.Cash)
            {
                ClearCurrentPaymentSum();
            }
        }).DisposeWith(disposables);

        BankCardVm.RemoveItemCommand.Subscribe(_ =>
        {
            if (CurrentPaymentType == PaymentType.BankCard)
            {
                ClearCurrentPaymentSum();
            }

        }).DisposeWith(disposables);

        CashlessPaymentVm.RemoveItemCommand.Subscribe(_ =>
        {
            if (CurrentPaymentType == PaymentType.CashlessPayment)
            {
                ClearCurrentPaymentSum();
            }

        }).DisposeWith(disposables);

        WithoutRevenueVm.RemoveItemCommand.Subscribe(_ =>
        {
            if (CurrentPaymentType == PaymentType.WithoutRevenue)
            {
                ClearCurrentPaymentSum();
            }

        }).DisposeWith(disposables);

        return ValueTask.CompletedTask;
    }

    protected void ClearCurrentPaymentSum()
    {
        CurrentPaymentSumText = string.Empty;
        _isFloat = false;
        _afterSeparatorDigitCount = 0;
    }

    protected void SetDisplayText(double value)
    {
        var text = ToDisplayDouble(value);
        _isFloat = IsFloat(value);
        _afterSeparatorDigitCount = (byte)(_isFloat ? CountDigitAfterSeparator(text) : 0);
        CurrentPaymentSumText = text;
    }

    protected static byte CountDigitAfterSeparator(string value)
    {
        var index = value.IndexOf(',');
        if (index == -1)
        {

            return 0;
        }

        return (byte)(value.Length - index - 1);
    }

    protected static bool IsFloat(double value)
    {
        return Math.Round(value, 3) % 1 != 0;
    }

    protected static string ToDisplayDouble(double value)
    {
        if (!IsFloat(value))
        {
            return value.ToString("F0");
        }
        return value.ToString("F2").Replace('.', ',');
    }
}
