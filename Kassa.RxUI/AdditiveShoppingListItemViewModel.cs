using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class AdditiveShoppingListItemViewModel : ReactiveObject, IShoppingListItem, IApplicationModelPresenter<AdditiveShoppingListItemDto>
{

    public Guid Id
    {
        get;
    }

    public AdditiveShoppingListItemDto Source
    {
        get => _source;
        set
        {
            _source = value;
            this.RaisePropertyChanged();
        }
    }
    private AdditiveShoppingListItemDto _source;
    private readonly CompositeDisposable _disposables = [];

    public AdditiveShoppingListItemViewModel(AdditiveShoppingListItemDto additive, IApplicationModelManager<AdditiveShoppingListItemDto> modelManager)
    {
        Id = additive.Id;
        _source = additive;

        this.WhenAnyValue(x => x.Source)
            .Subscribe(Update)
            .DisposeWith(_disposables);

        modelManager.AddPresenter(this)
            .DisposeWith(_disposables);

        RemoveCommand = ReactiveCommand.Create(() =>
        {
            modelManager.Remove(additive.Id);
        });
    }

    [Reactive]
    public string Name
    {
        get; set;
    } = null!;
    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;
    [Reactive]
    public double Price
    {
        get; set;
    }
    [Reactive]
    public double Count
    {
        get; set;
    }
    [Reactive]
    public string Measure
    {
        get; set;
    } = null!;
    [Reactive]
    public int Portion
    {
        get; set;
    }

    [Reactive]
    public bool IsSelected
    {
        get; set;
    }
    [Reactive]
    public double Discount
    {
        get;
        set;
    }
    [Reactive]
    public bool HasDiscount
    {
        get;
        set;
    }
    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get;
        set;
    } = null!;
    [Reactive]
    public double SubtotalSum
    {
        get;
        set;
    }

    [Reactive]
    public double TotalSum
    {
        get;
        set;
    }

    public IShoppingListItemDto SourceDto => Source;
    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    public void Dispose() => _disposables.Dispose();

    public void ModelChanged(Change<AdditiveShoppingListItemDto> change)
    {
        var current = change.Current;

        Update(current);
    }

    private void Update(AdditiveShoppingListItemDto item)
    {
        Name = item.Name;
        CurrencySymbol = item.CurrencySymbol;
        Price = item.Price;
        Count = item.Count;
        Measure = item.Measure;
        Portion = item.Portion;
        IsSelected = item.IsSelected;
    }
}
