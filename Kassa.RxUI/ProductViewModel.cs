using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class ProductViewModel : ProductHostItemVm, IApplicationModelPresenter<ProductDto>
{
    public static readonly ReactiveCommand<ProductViewModel, Unit> AddToShoppingListCommand = ReactiveCommand.CreateFromTask<ProductViewModel>(AddToShoppingList);

    private static async Task AddToShoppingList(ProductViewModel product)
    {
        var productDto = product._product;
        var isStopList = product._orderEditVm.IsStopList;


        if (isStopList)
        {
            productDto.IsAvailable = !productDto.IsAvailable;
            product._productService.RuntimeProducts.AddOrUpdate(productDto);
            return;
        }

        if (!product.IsAvailable)
        {
            return;
        }

        await product._orderEditVm.ShoppingList.AddProductShoppingListItem(product._product);
    }

    private ProductDto _product;

    private readonly CompositeDisposable _disposables = [];
    private readonly IOrderEditVm _orderEditVm;
    private readonly IProductService _productService;

    public ProductViewModel(IOrderEditVm orderEditVm, IProductService productService, ProductDto product)
    {
        _orderEditVm = orderEditVm;
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
        _product = product;
        HasIcon = product.Image >= 0;

        productService.RuntimeProducts.AddPresenter(this).DisposeWith(_disposables);
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
    public bool HasIcon
    {
        get; set;
    }

    public override void Dispose()
    {
        _disposables.Dispose();
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
        HasIcon = product.Image >= 0;

        _product = product;
    }
}