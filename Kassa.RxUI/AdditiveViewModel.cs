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
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class AdditiveViewModel : ReactiveObject, IReactiveToChangeSet<Guid, AdditiveDto>, IApplicationModelPresenter<AdditiveDto>
{
    public static readonly ReactiveCommand<AdditiveViewModel, Unit> AddAdditveToProduct = ReactiveCommand.CreateFromTask<AdditiveViewModel>(async additive =>
    {
        if (!additive.IsAvailable)
        {
            return;
        }

        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var additiveService = await Locator.Current.GetInitializedService<IAdditiveService>();
        var order = cashierService.CurrentOrder;

        if (order is null)
        {
            throw new InvalidOperationException("Order is not initialized");
        }


        await order.AddAdditiveToSelectedProducts(additive.Id);

        var additiveDto = await additiveService.GetAdditiveById(additive.Id);

        if (additiveDto is null)
        {
            throw new InvalidOperationException("Additive not found");
        }
        additive.IsAdded = order.IsAdditiveAdded(additive.Id);
    });

    private readonly CompositeDisposable _disposables = [];

    public AdditiveViewModel(AdditiveDto additive, IOrderEditService order, IAdditiveService additiveService)
    {
        Id = additive.Id;

        Name = additive.Name;
        СurrencySymbol = additive.CurrencySymbol;
        Price = additive.Price;
        Portion = additive.Portion;
        Measure = additive.Measure;
        IsAvailable = additive.IsAvailable && additive.IsEnoughIngredients;
        AddToShoppingListCommand = ReactiveCommand.Create(() => { });

        Source = additive;

        this.WhenAnyValue(x => x.Source)
            .Subscribe(UpdateSource)
            .DisposeWith(_disposables);

        if (order.IsInitialized)
        {
            IsAdded = order.IsAdditiveAdded(additive.Id);
        }

        AddToShoppingListCommand.DisposeWith(_disposables);

        additiveService.RuntimeAdditives.AddPresenter(this).DisposeWith(_disposables);
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

    public void ModelChanged(Change<AdditiveDto> change)
    {
        var additive = change.Current;

        UpdateSource(additive);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }


    private void UpdateSource(AdditiveDto additive)
    {
        Name = additive.Name;
        СurrencySymbol = additive.CurrencySymbol;
        Price = additive.Price;
        Portion = additive.Portion;
        Measure = additive.Measure;
        IsAvailable = additive.IsAvailable && additive.IsEnoughIngredients;
    }
}
