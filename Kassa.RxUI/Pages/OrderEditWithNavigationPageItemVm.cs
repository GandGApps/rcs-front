using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;

namespace Kassa.RxUI.Pages;
public sealed class OrderEditWithNavigationPageItemVm: BaseOrderEditPageVm
{
    public OrderEditWithNavigationPageItemVm(OrderEditDto orderEditDto,
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
