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
    public static readonly ReactiveCommand<CategoryDto, Unit> MoveToCategoryCommand = ReactiveCommand.CreateFromTask<CategoryDto>(async (category) =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.SelectCategory(category.Id);
    });

    public static readonly ReactiveCommand<Unit, Unit> NavigateBackCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();

        await cashierService.SelectPreviosCategory();
    });

    private readonly CategoryDto _category;
    public CategoryViewModel(CategoryDto category)
    {
        _category = category;
    }

    public int Id => _category.Id;

    public string Name => _category.Name;
}
