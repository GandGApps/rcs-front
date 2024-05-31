using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using DynamicData;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.Services;
public interface IOrderEditService : IInitializableService, INotifyPropertyChanged
{
    public ICategoryDto? CurrentCategory
    {
        get;
    }

    public IReadOnlyList<ICategoryDto> CategoriesStack
    {
        get;
    }

    public bool IsMultiSelect
    {
        get; set;
    }
    int? SelectedFavourite
    {
        get;
    }

    public double Discount
    {
        get;
    }

    public Guid OrderId
    {
        get;
    }

    public bool IsDelivery
    {
        get;
    }

    /// <summary>
    /// Bind the current category to ObservableCollection, that will be updated when the category changes.
    /// <para>
    /// It's need only for cashier page.
    /// </para>
    /// </summary>
    /// <param name="categoryItems"></param>
    /// <returns></returns>
    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItemDto> categoryItems);
    public IDisposable BindShoppingListItems<T>(Func<ProductShoppingListItemDto, IApplicationModelManager<ProductShoppingListItemDto>, T> creator, out ReadOnlyObservableCollection<T> shoppingListItems) where T : class, IModel;
    public IDisposable BindSelectedShoppingListItems(out ReadOnlyObservableCollection<IShoppingListItemDto> shoppingListItems);
    public IDisposable BindAdditivesForSelectedProduct<T>(Func<AdditiveDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<Guid, AdditiveDto>;
    public IDisposable BindAdditivesForProductShoppingListItem<T>(ProductShoppingListItemDto item, Func<AdditiveShoppingListItemDto, IApplicationModelManager<AdditiveShoppingListItemDto>, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IApplicationModelPresenter<AdditiveShoppingListItemDto>;
    public IDisposable BindFastMenu<T>(Func<FastMenuDto, IApplicationModelPresenter<FastMenuDto>> creator, out ReadOnlyObservableCollection<T> items);
    public Task AddProductToShoppingList(Guid productId);

    public Task SelectCategory(Guid categoryId);
    public Task SelectShoppingListItem(IShoppingListItemDto shoppingListItemDto);
    public Task UnselectShoppingListItem(IShoppingListItemDto shoppingListItemDto);

    /// <summary>
    /// Select the previos category in the list.
    /// </summary>
    /// <returns> true if the previous category exists</returns>
    public ValueTask<bool> SelectPreviosCategory();

    public ValueTask SelectRootCategory();

    public Task AddAdditiveToSelectedProducts(Guid additiveId);
    public Task WriteCommentToSelectedItems(string? comment);
    public Task WriteTotalComment(string? comment);
    public bool IsAdditiveAdded(Guid additiveId);

    public Task IncreaseSelectedProductShoppingListItem(double count = 1);
    public Task DecreaseSelectedProductShoppingListItem(double count = 1);

    public Task IncreaseProductShoppingListItem(ProductShoppingListItemDto item, double count = 1);
    public Task DecreaseProductShoppingListItem(ProductShoppingListItemDto item, double count = 1);

    public Task RemoveSelectedProductShoppingListItem();
    public ValueTask SelectFavourite(int favourite);

    public ValueTask<OrderDto> GetOrder();
    public Task AddProductToShoppingList(OrderedProductDto orderedProduct);
}