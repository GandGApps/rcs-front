using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class ProductViewModel : ReactiveObject, IActivatableViewModel, IApplicationModelPresenter<ProductDto>
{
    public static readonly ReactiveCommand<ProductViewModel, Unit> AddToShoppingListCommand = ReactiveCommand.CreateFromTask<ProductViewModel>(async product =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var order = cashierService.CurrentOrder;

        if (order is null)
        {
            throw new InvalidOperationException("Order is not selected");
        }

        if (!product.IsAvailable)
        {
            return;
        }
        await order.AddProductToShoppingList(product.Id);
    });

    private readonly IDisposable _disposable = Disposable.Empty;
    private readonly IOrderEditService _orderEditService;
    private readonly IProductService _productService;

    public ProductViewModel(IOrderEditService orderEditService, IProductService productService, ProductDto product)
    {
        _orderEditService = orderEditService;
        _productService = productService;

        Id = product.Id;
        Name = product.Name;
        CurrencySymbol = product.CurrencySymbol;
        Price = product.Price;
        IsAdded = product.IsAdded;
        Measure = product.Measure;
        IsAvailable = product.IsAvailable && product.IsEnoughIngredients;
        Image = product.Image;
        Color = product.Color;

        _disposable = productService.RuntimeProducts.AddPresenter(this);

        this.WhenActivated(disposables =>
        {
            _orderEditService.ShowPrice
                .ToPropertyEx(this, x => x.IsPriceVisible)
                .DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator
    {
        get;
    } = new();

    public Guid Id
    {
        get;
    }

    [Reactive]
    public string Name
    {
        get; set;
    }

    [Reactive]
    public string CurrencySymbol
    {
        get; set;
    }

    public extern bool IsPriceVisible
    {
        [ObservableAsProperty]
        get;
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
    public int Image
    {
        get; set;
    }

    [Reactive]
    public string Color
    {
        get; set;
    } 

    public void Dispose()
    {
        _disposable.Dispose();
        Activator.Dispose();
    }

    public void ModelChanged(Change<ProductDto> change)
    {
        var product = change.Current;

        Name = product.Name;
        CurrencySymbol = product.CurrencySymbol;
        Price = product.Price;
        IsAdded = product.IsAdded;
        Measure = product.Measure;
        IsAvailable = product.IsAvailable && product.IsEnoughIngredients;
        Image = product.Image;
        Color = product.Color;
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
public class DesignerProductViewModel : ReactiveObject
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
    public bool HasIcon
    {
        get; set;
    }

    [Reactive]
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
    {
        get; set;
    } = null!;

    [Reactive]
    public string[] Categories
    {
        get; set;
    } = [];
}
