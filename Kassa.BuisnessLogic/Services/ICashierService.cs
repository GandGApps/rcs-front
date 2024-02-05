using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using DynamicData;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic.Services;
public interface ICashierService : IInitializableService, INotifyPropertyChanged
{
    public CategoryDto? CurrentCategory
    {
        get;
    }

    public IReadOnlyList<CategoryDto> CategoriesStack
    {
        get;
    }

    public bool IsMultiSelect
    {
        get; set;
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
    public IDisposable BindShoppingListItems<T>(Func<ProductShoppingListItemDto, T> creator, out ReadOnlyObservableCollection<T> shoppingListItems) where T : class, IReactiveToChangeSet<int, ProductShoppingListItemDto>;
    public IDisposable BindSelectedShoppingListItems(out ReadOnlyObservableCollection<IShoppingListItemDto> shoppingListItems);
    public IDisposable BindAdditivesForSelectedProduct<T>(Func<AdditiveDto,T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<int, AdditiveDto>;
    public IDisposable BindAdditivesForProductShoppingListItem<T>(ProductShoppingListItemDto item, Func<AdditiveShoppingListItemDto, T> creator, out ReadOnlyObservableCollection<T> additives) where T : class, IReactiveToChangeSet<Guid, AdditiveShoppingListItemDto>;

    public Task AddProductToShoppingList(int productId);

    public Task SelectCategory(int categoryId);
    public Task SelectShoppingListItem(IShoppingListItemDto shoppingListItemDto);
    public Task UnselectShoppingListItem(IShoppingListItemDto shoppingListItemDto);

    /// <summary>
    /// Select the previos category in the list.
    /// </summary>
    /// <returns> true if the previous category exists</returns>
    public ValueTask<bool> SelectPreviosCategory();

    public Task AddAdditiveToProduct(int additiveId);
    public Task WriteCommentToSelectedItems(string? comment);
    public bool IsAdditiveAdded(int additiveId);
}