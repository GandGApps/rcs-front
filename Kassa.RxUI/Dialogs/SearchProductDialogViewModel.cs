using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Kassa.BuisnessLogic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI.Dialogs;
public class SearchProductDialogViewModel : DialogViewModel
{
    private readonly ICashierService _cashierService = Locator.Current.GetRequiredService<ICashierService>();

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

    [Reactive]
    public IList<ProductViewModel> FilteredProducts
    {
        get; private set;
    } = [];

    private ReadOnlyObservableCollection<ProductViewModel>? _products;
    public ReadOnlyObservableCollection<ProductViewModel>? Products => _products;

    protected override void OnActivated(CompositeDisposable disposables)
    {
        _cashierService.RuntimeProducts.Connect()
            .Transform(x =>
            {
                var vm = new ProductViewModel(x);

                vm.AddToShoppingListCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    SelectedProduct = vm;

                    await CloseAsync();
                });

                return vm;
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _products)
            .Subscribe()
            .DisposeWith(disposables);

        var sharedTextSearch = this.WhenAnyValue(x => x.SearchedText)
            .Skip(2) // fixing first blinking
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Publish()
            .RefCount();

        sharedTextSearch.FirstAsync().Subscribe(text =>
        {
            FilteredProducts = new ObservableCollection<ProductViewModel>();

            if (string.IsNullOrWhiteSpace(text))
            {
                FilteredProducts.Clear();
                FilteredProducts.AddRange(Products == null ? Array.Empty<ProductViewModel>() : Products);
            }
            else
            {
                FilteredProducts.Clear();
                FilteredProducts.AddRange(Products?.Where(x => x.Name.Contains(text, StringComparison.OrdinalIgnoreCase)) ?? []);
            }
        })
        .DisposeWith(disposables);

        sharedTextSearch.Subscribe(text =>
        {

            if (string.IsNullOrWhiteSpace(text))
            {
                FilteredProducts.Clear();
                FilteredProducts.AddRange(Products == null ? Array.Empty<ProductViewModel>() : Products);
            }
            else
            {
                FilteredProducts.Clear();
                FilteredProducts.AddRange(Products?.Where(x => x.Name.Contains(text, StringComparison.OrdinalIgnoreCase)) ?? []);
            }
        })
        .DisposeWith(disposables);

        FilteredProducts = _products;
    }
}
