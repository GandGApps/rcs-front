using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;

namespace Kassa.RxUI;
public class AdditiveShoppingListItemViewModel : ReactiveObject, IReactiveToChangeSet<Guid, AdditiveShoppingListItemDto>
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

    public AdditiveShoppingListItemViewModel(AdditiveShoppingListItemDto additive)
    {
        _source = additive;

        this.WhenAnyValue(x => x.Source)
            .Subscribe(Update);
    }

    public string Name
    {
        get; set;
    } = null!;

    public string CurrencySymbol
    {
        get; set;
    } = null!;

    public double Price
    {
        get; set;
    }

    public double Count
    {
        get; set;
    }

    public string Measure
    {
        get; set;
    } = null!;

    public int Portion
    {
        get; set;
    }

    private void Update(AdditiveShoppingListItemDto item)
    {
        Name = item.Name;
        CurrencySymbol = item.CurrencySymbol;
        Price = item.Price;
        Count = item.Count;
        Measure = item.Measure;
        Portion = item.Portion;
    }
}
