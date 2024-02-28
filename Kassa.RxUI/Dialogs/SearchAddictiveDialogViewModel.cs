using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;

public class SearchAddictiveDialogViewModel : DialogViewModel
{
    private readonly IAdditiveService _additiveService;
    private readonly IOrder _order;

    public SearchAddictiveDialogViewModel(IAdditiveService additiveService, IOrder order)
    {
        IsKeyboardVisible = false;
        _additiveService = additiveService;
        _order = order;
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {

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
            .Select(text => new Func<AdditiveDto, bool>(additive =>
                string.IsNullOrWhiteSpace(text) || additive.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase)));



        _additiveService.RuntimeAdditives
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(searchFilter)
            .TransformWithInlineUpdate(additive => new AdditiveViewModel(additive, _order), (vm,source) => vm.Source = source)
            .Bind(out _filteredAdditives)
            .Subscribe()
            .DisposeWith(disposables);

        searchTextStream.Connect().DisposeWith(disposables);
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

    public ReadOnlyObservableCollection<AdditiveViewModel> FilteredAddcitves => _filteredAdditives;
    private ReadOnlyObservableCollection<AdditiveViewModel> _filteredAdditives = ReadOnlyObservableCollection<AdditiveViewModel>.Empty;

    /// <summary>
    /// if it's null then user canceled dialog
    /// </summary>
    [Reactive]
    public AdditiveViewModel? SelectedAddictve
    {
        get; set;
    }
}
