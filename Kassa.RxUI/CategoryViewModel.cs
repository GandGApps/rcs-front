using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class CategoryViewModel : ProductHostItemVm, IActivatableViewModel, IApplicationModelPresenter<CategoryDto>
{
    public static readonly ReactiveCommand<CategoryViewModel, Unit> MoveToCategoryCommand = ReactiveCommand.CreateFromTask<CategoryViewModel>(async (category) =>
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

    private readonly Guid _categoryId;


    public CategoryViewModel(IApplicationModelManager<CategoryDto> modelManager, CategoryDto category)
    {
        _categoryId = category.Id;

        Name = category.Name;
        HasIcon = category.HasIcon;
        Image = category.Image;
        Color = category.Color;

        this.WhenActivated(disposables =>
        {
            modelManager.AddPresenter(this)
                .DisposeWith(disposables);
        });
    }

    public Guid Id => _categoryId;

    public ViewModelActivator Activator
    {
        get;
    } = new();

    [Reactive]
    public bool HasIcon
    {
        get; private set;
    }

    public void ModelChanged(Change<CategoryDto> change)
    {
        var category = change.Current;

        Name = category.Name;
        HasIcon = category.HasIcon;
        Image = category.Image;
        Color = category.Color;
    }

    public void Dispose()
    {
        Activator.Dispose();
    }
}
