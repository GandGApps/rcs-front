using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive.Linq;

namespace Kassa.RxUI.Dialogs;

/// <summary>
/// It's just non generic base class for select dialog.
/// Don't use it directly. Use <see cref="BaseSelectDialogViewModel{TItem,TVm}"/>
/// </summary>
public abstract class BaseSelectDialogViewModel: DialogViewModel
{
    protected static readonly TimeSpan SearchThrottle = TimeSpan.FromMilliseconds(500);

    [Reactive]
    public string? SearchText
    {
        get; set;
    }

    [Reactive]
    public bool IsKeyboardVisible
    {
        get; set;
    }

    public abstract IEnumerable? Items
    {
        get;
    }
}

public abstract class BaseSelectDialogViewModel<TItem, TVm> : BaseSelectDialogViewModel
    where TItem : class
    where TVm : class
{
    private readonly ObservableAsPropertyHelper<IEnumerable?> _items;

    public BaseSelectDialogViewModel()
    {
        this.WhenAnyValue(x => x.FilteredItems)
            .Select(items => (IEnumerable?)items)
            .ToProperty(this, x => x.Items, out _items)
            .DisposeWith(InternalDisposables);
    }

    public override IEnumerable? Items => _items.Value;
    
    [Reactive]
    public ReadOnlyObservableCollection<TVm>? FilteredItems
    {
        get; protected set;
    }

    [Reactive]
    public ReactiveCommand<TVm, Unit>? SelectCommand
    {
        get; protected set;
    }

    [Reactive]
    public TVm? SelectedItem
    {
        get; set;
    }
}