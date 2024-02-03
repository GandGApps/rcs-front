using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Dialogs;
public class SearchProductDialogViewModel : DialogViewModel
{
    private IProductService _productService;

    [Reactive]
    public ProductViewModel? SelectedProduct
    {
        get; set;
    }

    [Reactive]
    public string SearchedText
    {
        get; set;
    } = null!;

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    public ReadOnlyObservableCollection<ProductDto>? FilteredProducts => _filteredProducts;
    private ReadOnlyObservableCollection<ProductDto>? _filteredProducts;

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        _productService = await GetInitializedService<IProductService>();

        // Splitting the search text stream into immediate first item and throttled subsequent items
        var searchTextStream = this.WhenAnyValue(x => x.SearchedText).Publish();

        var firstItemStream = searchTextStream
            .Take(1)
            .ObserveOn(RxApp.MainThreadScheduler);

        var throttledStream = searchTextStream
            .Skip(1)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Merge(firstItemStream) // Merging the immediate first item with the throttled stream
            .DistinctUntilChanged();

        var searchFilter = throttledStream
            .Select(text => new Func<ProductDto, bool>(product =>
                string.IsNullOrWhiteSpace(text) || product.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase)));

        _productService.RuntimeProducts
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(searchFilter)
            .Bind(out _filteredProducts)
            .Subscribe()
            .DisposeWith(disposables);

        searchTextStream.Connect().DisposeWith(disposables);
    }

}
