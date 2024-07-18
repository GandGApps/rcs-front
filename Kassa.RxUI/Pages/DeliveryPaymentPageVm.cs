using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using DynamicData.Binding;
using DynamicData;
using Kassa.RxUI.Dialogs;
using System.Reactive.Linq;
using Kassa.DataAccess.Model;

namespace Kassa.RxUI.Pages;
public sealed class DeliveryPaymentPageVm(IPaymentService cashierPaymentService) : BasePaymentPageVm(cashierPaymentService)
{
}
