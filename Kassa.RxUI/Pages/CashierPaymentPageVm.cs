﻿using System;
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
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class CashierPaymentPageVm(ReadOnlyObservableCollection<ProductShoppingListItemViewModel> shoppingListItems,
        IOrderEditVm orderEditVm,
        IPaymentService paymentService,
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        IIngridientsService ingridientsService,
        IReceiptService receiptService,
        ICategoryService categoryService) :
    BasePaymentPageVm(shoppingListItems, orderEditVm, paymentService, cashierService, additiveService, productService, ingridientsService, receiptService, categoryService)
{
}
