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
public sealed class DeliveryPaymentPageVm(ReadOnlyObservableCollection<ProductShoppingListItemViewModel> shoppingListViewModels,
        IOrderEditVm orderEditVm,
        IPaymentService paymentService,
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        IIngridientsService ingridientsService,
        IReceiptService receiptService,
        ICategoryService categoryService) :
    BasePaymentPageVm(shoppingListViewModels, orderEditVm, paymentService, cashierService, additiveService, productService, ingridientsService, receiptService, categoryService)
{
}
