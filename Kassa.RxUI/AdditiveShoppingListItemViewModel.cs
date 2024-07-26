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
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class AdditiveShoppingListItemViewModel : ReactiveObject, IShoppingListItemVm, IApplicationModelPresenter<AdditiveDto>
{

    public Guid Id
    {
        get;
    }

    private readonly CompositeDisposable _disposables = [];
    private AdditiveDto _additiveDto;

    public AdditiveShoppingListItemViewModel(OrderedAdditiveDto orderedAdditiveDto, AdditiveDto additive, IApplicationModelManager<AdditiveDto> modelManager, IOrderEditVm orderEditVm)
        : this(additive, modelManager, orderEditVm)
    {
        OrderedId = orderedAdditiveDto.Id;
    }

    public AdditiveShoppingListItemViewModel(AdditiveDto additive, IApplicationModelManager<AdditiveDto> modelManager, IOrderEditVm orderEditVm)
    {
        _additiveDto = additive;
        Id = additive.Id;


        modelManager.AddPresenter(this)
            .DisposeWith(_disposables);

        Update(additive);

        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(x =>
            {
                if (x)
                {
                    orderEditVm.ShoppingList.SelectedItems.Add(this);
                }
                else
                {
                    orderEditVm.ShoppingList.SelectedItems.Remove(this);
                }
            })
            .DisposeWith(_disposables);

        RemoveCommand = ReactiveCommand.Create(() =>
        {
            modelManager.Remove(additive.Id);
        }).DisposeWith(_disposables);
    }

    public AdditiveDto AdditiveDto => _additiveDto;

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
    } = 1;

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
        get; set;
    }

    [Reactive]
    public bool HasDiscount
    {
        get; set;
    }

    public Guid OrderedId
    {
        get; set;
    }

    [Reactive]
    public ShoppingListViewModel ShoppingListViewModel
    {
        get; set;
    } = null!;

    [Reactive]
    public double SubtotalSum
    {
        get; set;
    }

    [Reactive]
    public double TotalSum
    {
        get;
        set;
    }

    public ReactiveCommand<Unit, Unit> RemoveCommand
    {
        get;
    }

    public void Dispose() => _disposables.Dispose();

    public void ModelChanged(Change<AdditiveDto> change)
    {
        var current = change.Current;

        _additiveDto = current;

        Update(current);
    }

    private void Update(AdditiveDto item)
    {
        Name = item.Name;
        CurrencySymbol = item.CurrencySymbol;
        Price = item.Price;
        Measure = item.Measure;
        Portion = item.Portion;
    }
}
