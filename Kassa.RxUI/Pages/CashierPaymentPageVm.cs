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
    public CashierPaymentPageVm(MainViewModel mainViewModel) : base(mainViewModel)
    {
        SendReceiptCommand = ReactiveCommand.Create(() => { });
        EnableWithCheckboxCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!WithReceipt)
            {
                await mainViewModel.DialogOpenCommand
                    .Execute(new SendReceiptDialogViewModel(mainViewModel, this))
                    .FirstAsync();
            }
            else
            {
                IsEmail = false;
                IsPrinter = false;
            }
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

        SetPaymentTypeCommand = ReactiveCommand.Create<PaymentType>(type => CurrentPaymentType = type);

        AddPaymentItemCommand = ReactiveCommand.Create(() =>
        {
            var paymentItem = new CashierPaymentItemVm();
            paymentItem.RemoveItem = ReactiveCommand.Create(() => { CashierPaymentItemVms.Remove(paymentItem); });
            paymentItem.Name = CurrentPaymentType switch
            {
                PaymentType.Cash => "Наличные",
                PaymentType.BankCard => "Банковская карта",
                PaymentType.CashlessPayment => "Безналичный расчет",
                PaymentType.WithoutRevenue => "Без выручки",
                _ => "Неизвестный тип оплаты"
            };
            paymentItem.Cost = CurrentPaymentSum;

            CashierPaymentItemVms.Add(paymentItem);

            ClearCurrentPaymentSum();
        });
    }

    [Reactive]
    public ICashierPaymentService CashierPaymentService
    {
        get; private set;
    } = null!;

    [Reactive]
    public ReadOnlyObservableCollection<ProductShoppingListItemViewModel>? ShoppingListItems
    {
        get; set;
    }

    public ReactiveCommand<PaymentType, Unit> SetPaymentTypeCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> AddPaymentItemCommand
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

    public extern bool WithReceipt
    {
        [ObservableAsProperty]
        get;
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


    public ReactiveCommand<double, Unit> PlusCommand
    {
        get;
    }

    public ReactiveCommand<string, Unit> AddDigitCommand
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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        CashierPaymentService = await GetInitializedService<ICashierPaymentService>();

        CashierPaymentService.CashierService
            .BindShoppingListItems(x => new ProductShoppingListItemViewModel(x), out var shoppingListItems)
            .DisposeWith(disposables);

        ShoppingListItems = shoppingListItems;

        ShoppingListItems.ToObservableChangeSet()
                         .AutoRefresh(x => x.SubtotalSum)
                         .ToCollection()
                         .Select(x => x.Sum(x => x.SubtotalSum))
                         .Subscribe(x => Subtotal = x)
                         .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) => email || printer)
            .ToPropertyEx(this, x => x.WithReceipt)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Subtotal)
            .Subscribe(x => Total = x)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.IsEmail, x => x.IsPrinter, (email, printer) =>
            {
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


        this.WhenAnyValue(x => x.CashierPaymentItemVms, (paymentItems) => paymentItems.Sum(x => x.Cost))
            .ToPropertyEx(this, x => x.ToEntered)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEntered, x => x.Total, (entered, total) => Math.Max(0, total - entered))
            .ToPropertyEx(this, x => x.ToEnter)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.ToEnter, x => x.ToEntered, (toEnter, entered) => Math.Max(0, entered - toEnter))
            .ToPropertyEx(this, x => x.Change)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.CurrentPaymentSumText, (text) =>
            {
                text = text.Replace(',', '.');
                return string.IsNullOrWhiteSpace(text) ? 0 : text.EndsWith('.') ? double.Parse(text[..^1]) : double.Parse(text);
            })
            .ToPropertyEx(this, x => x.CurrentPaymentSum)
            .DisposeWith(disposables);

    }

    private void ClearCurrentPaymentSum()
    {
        CurrentPaymentSumText = string.Empty;
        _isFloat = false;
        _afterSeparatorDigitCount = 0;
    }
}
