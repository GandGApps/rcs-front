using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public class CashierPaymentPageVm : PageViewModel
{

    private bool _isFloat;
    private byte _afterSeparatorDigitCount;
    public CashierPaymentPageVm(MainViewModel mainViewModel, ICashierPaymentService cashierPaymentService) : base(mainViewModel)
    {
        CashierPaymentService = cashierPaymentService;
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

        SendReceiptCommand = ReactiveCommand.Create(() => { });
        EnableWithCheckboxCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await mainViewModel.DialogOpenCommand
                    .Execute(new SendReceiptDialogViewModel(mainViewModel, this))
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
            if (CurrentPaymentType == PaymentType.Cash)
            {
                SetDisplayText(ToEnter);
            }
            else if (CurrentPaymentType == PaymentType.BankCard)
            {
                SetDisplayText(ToEnter);
            }
            else if (CurrentPaymentType == PaymentType.CashlessPayment)
            {
                SetDisplayText(ToEnter);
            }
            else if (CurrentPaymentType == PaymentType.WithoutRevenue)
            {
                SetDisplayText(ToEnter);
            }
        });

        PayCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var loading = new LoadingDialogViewModel(mainViewModel)
            {
                Message = "Оплата..."
            };

            mainViewModel.DialogOpenCommand.Execute(loading).Subscribe();

            await cashierPaymentService.Pay();

            await loading.CloseAsync();

            if (Change - 0.001 >= 0)
            {
                await mainViewModel.OkMessage("Сдача \n" + Change + " " + CurrencySymbol, "");
            }

            await mainViewModel.OkMessage("Оплата прошла успешно", "");
            await mainViewModel.GoToPageAndResetCommand.Execute(new MainPageVm(mainViewModel));


        }, this.WhenAnyValue(x => x.IsExactAmount));
    }

    public ICashierPaymentService CashierPaymentService
    {
        get;
    }

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get; set;
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
        get;
        set;
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

    [Reactive]
    public bool IsPrinter
    {
        get; set;
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
        get; private set;
    } = string.Empty;

    public extern double CurrentPaymentSum
    {
        [ObservableAsProperty]
        get;
    }

    [Reactive]
    private bool Locker
    {
        get; set;
    }

    public extern bool IsExactAmount
    {
        [ObservableAsProperty]
        get;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        CashierPaymentService.Order
            .BindShoppingListItems(x => new ProductShoppingListItemViewModel(x, CashierPaymentService.Order), out var shoppingListItems)
            .DisposeWith(disposables);

        ShoppingListItems = shoppingListItems;

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => Subtotal = x)
                         .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Subtotal)
            .Subscribe(x => Total = x)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, x => x.WithReceipt, (email, printer, withReceipt) =>
            {
                if (!withReceipt)
                {
                    return "Распечатать, переслать чек";
                }

                if (withReceipt)
                {
                    if (!email && !printer)
                    {
                        IsPrinter = true;
                        return "Переслать чек";
                    }
                }

                if (email)
                {
                    return "Отправить на почту";
                }
                else if (printer)
                {

                    return "Переслать чек";
                }
                else
                {
                    return "Распечатать, переслать чек";
                }
            })
            .ToPropertyEx(this, x => x.ReceiptActionText)
            .DisposeWith(disposables);

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
            .DisposeWith(disposables);


        CashierPaymentItemVms.ToObservableChangeSet()
            .AutoRefresh(x => x.Entered)
            .ToCollection()
            .Select(x => x.Sum(x => x.Entered))
            .ToPropertyEx(this, x => x.ToEntered)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEntered, x => x.Total, (entered, total) => Math.Max(0, total - entered))
            .ToPropertyEx(this, x => x.ToEnter)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEnter, x => x.ToEntered, x => x.Total, (toEnter, entered, total) => Math.Max(0, entered - total))
            .ToPropertyEx(this, x => x.Change)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.CurrentPaymentSumText, (text) =>
            {
                text = text.Replace(',', '.');
                return string.IsNullOrWhiteSpace(text) ? 0 : text.EndsWith('.') ? double.Parse(text[..^1]) : double.Parse(text);
            })
            .ToPropertyEx(this, x => x.CurrentPaymentSum)
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


        this.WhenAnyValue(x => x.Total, x => x.ToEntered, (total, entered) => total <= entered)
            .ToPropertyEx(this, x => x.IsExactAmount)
            .DisposeWith(disposables);
    }

    private void ClearCurrentPaymentSum()
    {
        CurrentPaymentSumText = string.Empty;
        _isFloat = false;
        _afterSeparatorDigitCount = 0;
    }

    private void SetDisplayText(double value)
    {
        var text = ToDisplayDouble(value);
        _isFloat = IsFloat(value);
        _afterSeparatorDigitCount = (byte)(_isFloat ? CountDigitAfterSeparator(text) : 0);
        CurrentPaymentSumText = text;
    }

    private static byte CountDigitAfterSeparator(string value)
    {
        var index = value.IndexOf(',');
        if (index == -1)
        {

            return 0;
        }

        return (byte)(value.Length - index - 1);
    }

    private static bool IsFloat(double value)
    {
        return Math.Round(value, 3) % 1 != 0;
    }

    private static string ToDisplayDouble(double value)
    {
        if (!IsFloat(value))
        {
            return value.ToString("F0");
        }
        return value.ToString("F2").Replace('.', ',');
    }
}
