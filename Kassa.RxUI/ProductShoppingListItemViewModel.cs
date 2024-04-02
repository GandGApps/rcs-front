using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public class ProductShoppingListItemViewModel : ReactiveObject, IShoppingListItem, IReactiveToChangeSet<Guid, ProductShoppingListItemDto>
{
    private readonly CompositeDisposable _disposables = [];

    public Guid Id
    {
        get; set;
    }

    public ProductShoppingListItemViewModel(ProductShoppingListItemDto product, IOrderEditService order)
    {
        Source = product;
        _source = product;

        if (order.IsInitialized)
        {
            order.BindAdditivesForProductShoppingListItem(product, (x,y) => new AdditiveShoppingListItemViewModel(x,y), out var additives)
                .DisposeWith(_disposables);
            Additives = additives;
        }
        else
        {
            Additives = new([]);
        }

        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(x =>
            {

            });

        this.WhenAnyValue(x => x.AdditiveInfo)
            .Select(x => !string.IsNullOrEmpty(x))
            .Subscribe(x => HasAdditiveInfo = x);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveSubtotalSum)
            .Select(x => (x.Item1 * x.Item2) + x.Item3)
            .Subscribe(x => SubtotalSum = x);

        this.WhenAnyValue(x => x.Count, x => x.Price, x => x.AddictiveTotalSum)
            .Select(x => ((x.Item1 * x.Item2) + x.Item3) * (1 -  Discount))
            .Subscribe(x => TotalSum = x);

        Additives
            .ToObservableChangeSet()
            .AutoRefresh(x => x.Price)
            .AutoRefresh(x => x.Count)
            .ToCollection()
            .Select(list => list.Sum(item => item.Price))
            .Subscribe(x => AddictiveSubtotalSum = x);

        Additives
            .ToObservableChangeSet()
            .ToCollection()
            .Select(list => list.Sum(item => item.Price))
            .Subscribe(x => AddictiveTotalSum = x);


        RemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {

        });
    }

    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get; set;
    } = null!;

    [Reactive]
    public bool HasDiscount
    {
        get; set;
    }

    [Reactive]
    public double Discount
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
    public string Name
    {
        get; set;
    } = null!;

    [Reactive]
    public double Price
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    } = null!;

    [Reactive]
    public string? AdditiveInfo
    {
        get; set;
    }

    public ReadOnlyObservableCollection<AdditiveShoppingListItemViewModel> Additives
    {
        get;
    }

    [Reactive]
    public bool HasAdditiveInfo
    {
        get; set;
    }
    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    [Reactive]
    public double AddictiveSubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double AddictiveTotalSum
    {
        get; set;
    }

    [Reactive]
    public double SubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double TotalSum
    {
        get; set;
    }
    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    public ProductShoppingListItemDto Source
    {
        get => _source;
        set
        {
            _source = value;
            this.RaisePropertyChanged();
            Update(value);
        }
    }
    private ProductShoppingListItemDto _source;

    public IShoppingListItemDto SourceDto => Source;

    private void Update(ProductShoppingListItemDto product)
    {
        Count = product.Count;
        Price = product.Price;
        Measure = product.Measure;
        Name = product.Name;
        CurrencySymbol = product.CurrencySymbol;
        Id = product.ItemId;
        IsSelected = product.IsSelected;
        AdditiveInfo = product.AdditiveInfo;
        HasAdditiveInfo = !string.IsNullOrEmpty(product.AdditiveInfo);
    }
}
