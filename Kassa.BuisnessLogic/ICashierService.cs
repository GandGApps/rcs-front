using System.Collections.ObjectModel;
using System.ComponentModel;
using DynamicData;
using Kassa.DataAccess;

namespace Kassa.BuisnessLogic;
public interface ICashierService : IInitializableService, INotifyPropertyChanged
{
    public Category? CurrentCategory
    {
        get;
    }

    public IReadOnlyList<Category> CategoriesStack
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
    public IDisposable BindSelectedCategoryItems(out ReadOnlyObservableCollection<ICategoryItem> categoryItems);

    public IDisposable BindShoppingListItems(out ReadOnlyObservableCollection<IShoppingListItem> shoppingListItems);
    public Task SelectCategory(int categoryId);

    /// <summary>
    /// Select the previos category in the list.
    /// </summary>
    /// <returns> true if the previous category exists</returns>
    public ValueTask<bool> SelectPreviosCategory();
}