using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Dialogs;
public class SearchProductDialogViewModel : ApplicationManagedModelSearchableDialogViewModel<ProductDto, ProductViewModel>
{
    private readonly IOrderEditService _orderEditService;
    private readonly IProductService _productService;

    public SearchProductDialogViewModel(IOrderEditService orderEditService, IProductService productService)
    {
        _orderEditService = orderEditService;
        this._productService = productService;
    }

    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        Filter(_productService.RuntimeProducts, x => new ProductViewModel(_orderEditService, _productService, x), disposables);
        return new ValueTask();
    }

    protected override bool IsMatch(string searchText, ProductDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
