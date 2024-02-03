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
public class AdditiveViewModel : ReactiveObject
{
    public static readonly ReactiveCommand<AdditiveDto, Unit> AddAdditveToProduct = ReactiveCommand.CreateFromTask<AdditiveDto>(async additive =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.AddAdditiveToProduct(additive.Id);
    });
    public AdditiveViewModel(AdditiveDto additive)
    {
        Name = additive.Name;
        СurrencySymbol = additive.CurrencySymbol;
        Price = additive.Price;
        Portion = additive.Portion;
        IsAdded = additive.IsAdded;
        Measure = additive.Measure;
        IsAvailable = additive.Count > 0;
        AddToShoppingListCommand = ReactiveCommand.Create(() => { });
    }

    public string Name
    {
        get;
    }

    public string СurrencySymbol
    {
        get;
    }
    public double Price
    {
        get;
    }

    public bool IsAdded
    {
        get;
    }

    public double Portion
    {
        get;
    }

    /// <summary>
    /// Measure of addictive, for example, kg, l, etc.
    /// </summary>
    public string Measure
    {
        get;
    }


    public bool IsAvailable
    {
        get;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    }
}
