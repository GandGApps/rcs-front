using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.DataAccess.Model;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Linq;
using DynamicData;

namespace Kassa.RxUI.Dialogs;
public abstract class ApplicationManagedModelSearchableDialogViewModel<TItem, TVm>: DialogViewModel
    where TItem : class, IModel
    where TVm : class, IApplicationModelPresenter<TItem>
{
    protected readonly TimeSpan SearchThrottle = TimeSpan.FromMilliseconds(500);
    protected readonly ObservableCollection<TVm> _filteredItems = new();

    public ApplicationManagedModelSearchableDialogViewModel()
    {
        FilteredItems = new ReadOnlyCollection<TVm>(_filteredItems);
    }

    [Reactive]
    public string? SearchText
    {
        get; set;
    }

    public ReadOnlyCollection<TVm>? FilteredItems
    {
        get;
    }

    [Reactive]
    public ReactiveCommand<TVm, Unit>? SelectCommand
    {
        get; protected init;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    [Reactive]
    public TVm? SelectedItem
    {
        get; set;
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