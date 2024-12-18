﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using DynamicData;
using Kassa.Shared;

namespace Kassa.RxUI.Dialogs;
public abstract class ApplicationManagedModelSearchableDialogViewModel<TItem, TVm>: BaseSelectDialogViewModel<TItem, TVm>
    where TItem : class, IGuidId
    where TVm : class, IApplicationModelPresenter<TItem>
{
    protected readonly ObservableCollection<TVm> _filteredItems = [];

    public ApplicationManagedModelSearchableDialogViewModel()
    {
        FilteredItems = new ReadOnlyObservableCollection<TVm>(_filteredItems);
    }


    protected void Filter(IApplicationModelManager<TItem> modelManager, Func<TItem, TVm> selector, CompositeDisposable disposables)
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

        searchFilter.Subscribe(filter =>
        {
            _filteredItems.Clear();
            _filteredItems.AddRange(modelManager.Values.Where(filter).Select(x =>
            {
                var itemVm = selector(x);
                modelManager.AddPresenter(itemVm).DisposeWith(disposables);
                return itemVm;
            }));
        }).DisposeWith(disposables);

        searchTextStream.Connect().DisposeWith(disposables);
    }

    protected virtual bool IsMatch(string searchText, TItem item)
    {
        return true;
    }
}