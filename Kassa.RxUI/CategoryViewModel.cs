using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using Splat;

namespace Kassa.RxUI;
public class CategoryViewModel : ReactiveObject
{
    public static readonly ReactiveCommand<CategoryDto, Unit> MoveToCategoryCommand = ReactiveCommand.CreateFromTask<CategoryDto>(async (category) =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var order = cashierService.CurrentOrder;

        if (order is null)
        {
            throw new InvalidOperationException("Order is not selected");
        }

        await order.SelectCategory(category.Id);
    });

    public static readonly ReactiveCommand<Unit, Unit> NavigateBackCategoryCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        var cashierService = await Locator.Current.GetInitializedService<ICashierService>();
        var order = cashierService.CurrentOrder;

        if (order is null)
        {
            throw new InvalidOperationException("Order is not selected");
        }

        await order.SelectPreviosCategory();
    });

    private readonly CategoryDto _category;
    public CategoryViewModel(CategoryDto category)
    {
        _category = category;
    }

    public Guid Id => _category.Id;

    public string Name => _category.Name;

    public bool HasIcon => _category.HasIcon;

    public string? Icon => _category.Icon;
}
