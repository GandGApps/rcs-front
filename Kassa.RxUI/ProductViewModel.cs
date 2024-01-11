using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class ProductViewModel : ReactiveObject, IActivatableViewModel
{
    public int Id
    {
        get; set;
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
    public bool IsAdded
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
    public bool IsAvailable
    {
        get; set;
    }

    [Reactive]
    public string Icon
    {
        get; set;
    } = null!;

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    } = null!;

    public ViewModelActivator Activator
    {
        get;
    }

    [Reactive]
    public string[] Categories
    {
        get; set;
    } = [];

    [EditorBrowsable(EditorBrowsableState.Never)]
    /// <summary>
    /// Needs for design-time data.
    /// Don't use it in code.
    /// </summary>
    public ProductViewModel()
    {
        Activator = new();

        this.WhenActivated(Activate);
    }

    public ProductViewModel(Product product)
    {
        Activator = new();

        Id = product.Id;
        Name = product.Name;
        CurrencySymbol = product.CurrencySymbol;
        Price = product.Price;
        Count = product.Count;
        Measure = product.Measure;
        Categories = product.Categories;
        Icon = product.Icon;

        this.WhenActivated(Activate);
    }

    private void Activate(CompositeDisposable disposables)
    {
        this.WhenAnyValue(x => x.Count)
            .Subscribe(x =>
            {
                if (x <= 0)
                {
                    IsAvailable = false;
                }
                else
                {
                    IsAvailable = true;
                }
            })
            .DisposeWith(disposables);
    }
}
