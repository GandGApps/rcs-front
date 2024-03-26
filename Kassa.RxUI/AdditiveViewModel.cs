using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public class AdditiveViewModel : ReactiveObject, IReactiveToChangeSet<Guid, AdditiveDto>
{
    public static readonly ReactiveCommand<AdditiveViewModel, Unit> AddAdditveToProduct = ReactiveCommand.CreateFromTask<AdditiveViewModel>(async additive =>
    {
        if (additive.Count <= 0)
        {
            return;
        }
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var order = cashierService.CurrentOrder;

        await order.AddAdditiveToSelectedProducts(additive.Id);

        additive.IsAdded = true;
        additive.Count--;
    });


    public AdditiveViewModel(AdditiveDto additive, IOrderEditService order)
    {
        Id = additive.Id;

        UpdateSource(additive);

        Source = additive;

        this.WhenAnyValue(x => x.Source)
            .Subscribe(UpdateSource);

        if (order.IsInitialized)
        {
            IsAdded = order.IsAdditiveAdded(additive.Id);
        }
    }


    [Reactive]
    public string Name
    {
        get; set;
    }

    [Reactive]
    public string СurrencySymbol
    {
        get; set;
    }
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
    public double Portion
    {
        get; set;
    }

    [Reactive]
    public double Count
    {
        get; set;
    }


    /// <summary>
    /// Measure of addictive, for example, kg, l, etc.
    /// </summary>
    [Reactive]
    public string Measure
    {
        get; set;
    }

    [Reactive]
    public bool IsAvailable
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    }
    public Guid Id
    {
        get;
    }

    [Reactive]
    public AdditiveDto Source
    {
        get;
        set;
    }

    private void UpdateSource(AdditiveDto additive)
    {
        Name = additive.Name;
        СurrencySymbol = additive.CurrencySymbol;
        Price = additive.Price;
        Portion = additive.Portion;
        Measure = additive.Measure;
        IsAvailable = additive.IsAvailable;
        AddToShoppingListCommand = ReactiveCommand.Create(() => { });
    }
}
