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
    private IAdditiveService _additiveService;

    public SearchAddictiveDialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        IsKeyboardVisible = false;
    }


    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        _additiveService = await GetInitializedService<IAdditiveService>();

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
            .Select(text => new Func<AdditiveDto, bool>(addictive =>
                           string.IsNullOrWhiteSpace(text) || addictive.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase)));

        _additiveService.RuntimeAdditives
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(searchFilter)
            .Bind(out _filteredAdditives)
            .DisposeMany()
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

    public ReadOnlyObservableCollection<AdditiveDto> FilteredAddcitves => _filteredAdditives;
    private ReadOnlyObservableCollection<AdditiveDto> _filteredAdditives;

    /// <summary>
    /// if it's null then user canceled dialog
    /// </summary>
    [Reactive]
    public AdditiveViewModel? SelectedAddictve
    {
        get; set;
    }
}
