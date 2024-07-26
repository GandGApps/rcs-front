using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.BuisnessLogic;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using DynamicData.Binding;
using Kassa.RxUI.Dialogs;
using System.Reactive.Linq;
using DynamicData;

namespace Kassa.RxUI.Pages;
public sealed class DeliveryOrderEditPageVm : BaseOrderEditPageVm
{
    public DeliveryOrderEditPageVm(OrderEditDto orderEditDto,
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
