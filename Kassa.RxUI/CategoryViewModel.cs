using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.DataAccess;
using ReactiveUI;
using Splat;

namespace Kassa.RxUI;
public class CategoryViewModel : ReactiveObject
{
    public static readonly ReactiveCommand<Category, Unit> MoveToCategoryCommand = ReactiveCommand.CreateFromTask<Category>(async (category) =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.SelectCategory(category.Id);
    });

    private readonly Category _category;
    public CategoryViewModel(Category category)
    {
        _category = category;
    }

    public int Id => _category.Id;

    public string Name => _category.Name;
}
