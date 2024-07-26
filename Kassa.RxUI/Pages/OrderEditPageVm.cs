using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
using Kassa.DataAccess;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Pages;
public sealed class OrderEditPageVm : BaseOrderEditPageVm
{
    public OrderEditPageVm(OrderEditDto orderEditDto,
        IStorageScope storageScope,
        ICashierService cashierService,
        IAdditiveService additiveService,
        IProductService productService,
        ICategoryService categoryService,
        IReceiptService receiptService,
        IIngridientsService ingridientsService) : base(orderEditDto, storageScope, cashierService, additiveService, productService, categoryService, receiptService, ingridientsService)
    {
    }
}
