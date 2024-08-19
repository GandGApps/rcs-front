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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;
public abstract class SearchableDialogViewModel<TItem, TVm> : BaseSelectDialogViewModel<TItem, TVm>
    where TItem : class
    where TVm : class
{
    protected void Filter<TKey>(SourceCache<TItem, TKey> sourceCache, Func<TItem, TVm> selector, Action<TVm, TItem> updater, CompositeDisposable disposables) where TKey : notnull

    {
        var searchTextStream = this.WhenAnyValue(x => x.SearchText).Publish();

        var firstItemStream = searchTextStream
            .Take(1)
            .ObserveOn(RxApp.MainThreadScheduler);

        var throttledStream = searchTextStream
            .Skip(1)
            .Throttle(SearchThrottle)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Merge(firstItemStream)
            .DistinctUntilChanged();

        var searchFilter = throttledStream
            .Select(text => new Func<TItem, bool>(item =>
                           string.IsNullOrWhiteSpace(text) || IsMatch(text, item)));

        sourceCache.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(searchFilter)
            .TransformWithInlineUpdate(selector, (vm, source) => updater(vm, source))
            .Bind(out var _filteredClients)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposables);

        searchTextStream.Connect().DisposeWith(disposables);

        FilteredItems = _filteredClients;
    }


    protected virtual bool IsMatch(string searchText, TItem item)
    {
        return true;
    }
}
