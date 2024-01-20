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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Dialogs;

public class SearchAddictiveDialogViewModel : DialogViewModel
{
    public SearchAddictiveDialogViewModel(MainViewModel mainViewModel) : base(mainViewModel)
    {
        IsKeyboardVisible = false;
    }

    protected override void OnActivated(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.SearchedText)
            .Skip(2) // fixing first blinking
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x =>
            {
                if (string.IsNullOrWhiteSpace(x))
                {
                    FilteredAddcitves.Clear();
                    FilteredAddcitves.AddRange(Addictives);
                    return;
                }
                var filtered = Addictives.Where(x => x.Name.Contains(SearchedText, StringComparison.CurrentCultureIgnoreCase));
                FilteredAddcitves.Clear();
                FilteredAddcitves.AddRange(filtered);
            })
            .DisposeWith(disposables);
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

    public ReadOnlyObservableCollection<AdditiveViewModel> Addictives => _addictives;
    private readonly ReadOnlyObservableCollection<AdditiveViewModel> _addictives;

    public ObservableCollection<AdditiveViewModel> FilteredAddcitves
    {
        get;
    } 

    /// <summary>
    /// if it's null then user canceled dialog
    /// </summary>
    [Reactive]
    public AdditiveViewModel? SelectedAddictve
    {
        get; set;
    }
}
