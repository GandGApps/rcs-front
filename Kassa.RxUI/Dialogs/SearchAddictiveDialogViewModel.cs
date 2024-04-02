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
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;

public class SearchAddictiveDialogViewModel : DialogViewModel
{
    private readonly IAdditiveService _additiveService;
    private readonly IOrderEditService _order;

    public SearchAddictiveDialogViewModel(IAdditiveService additiveService, IOrderEditService order)
    {
        IsKeyboardVisible = false;
        _additiveService = additiveService;
        _order = order;
    }


    protected override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        IDisposable? searchDisposable = null;

        var observableCollection = new ObservableCollection<AdditiveViewModel>();
        _filteredAdditives = new(observableCollection);

        this.WhenAnyValue(x => x.SearchedText)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(text =>
            {
                searchDisposable?.Dispose();
                searchDisposable = _additiveService.RuntimeAdditives
                    .Filter(additive => additive.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
                    .Subscribe(changeSet =>
                    {
                        foreach (var change in changeSet.Changes)
                        {
                            switch (change.Reason)
                            {
                                case ModelChangeReason.Add:
                                    observableCollection.Add(new AdditiveViewModel(change.Current, _order, _additiveService));
                                    break;
                                case ModelChangeReason.Remove:
                                    observableCollection.Remove(observableCollection.First(x => x.Id == change.Current.Id));
                                    break;
                            }
                        }
                    });

                var filteredAdditives = _additiveService.RuntimeAdditives.Values
                    .Where(additive => additive.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
                    .Select(additive => new AdditiveViewModel(additive, _order, _additiveService));

                observableCollection.Clear();
                observableCollection.AddRange(filteredAdditives);
            })
            .DisposeWith(disposables);

        return ValueTask.CompletedTask;
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
