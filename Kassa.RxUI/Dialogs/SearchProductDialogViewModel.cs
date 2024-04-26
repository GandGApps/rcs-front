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

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        var productService = await Locator.GetInitializedService<IProductService>();

        Filter(productService.RuntimeProducts, x => new ProductViewModel(x), disposables);
    }

    protected override bool IsMatch(string searchText, ProductDto item)
    {
        return item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
}
