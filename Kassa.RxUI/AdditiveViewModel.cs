using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class AdditiveViewModel : ReactiveObject
{

    public AdditiveViewModel(Additive additive, bool isAdded = false)
    {
        Name = additive.Name;
        СurrencySymbol = additive.CurrencySymbol;
        Price = additive.Price;
        Portion = additive.Portion;
        IsAdded = isAdded;
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
    public ReactiveCommand<Unit,Unit> AddToShoppingListCommand
    {
        get; set;
    }
}
