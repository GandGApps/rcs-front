using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
using Kassa.RxUI.Pages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class CategoryViewModel : ProductHostItemVm, IApplicationModelPresenter<CategoryDto>
{
    public static readonly ReactiveCommand<CategoryViewModel, Unit> MoveToCategoryCommand = ReactiveCommand.Create<CategoryViewModel>((category) =>
    {
        category._orderEditVm.MoveToCategoryUnsafe(category.Id);
    });

    private readonly IOrderEditVm _orderEditVm;

    private CategoryDto _category;
    private readonly IDisposable _disposable;

    public CategoryViewModel(IApplicationModelManager<CategoryDto> modelManager, CategoryDto category, IOrderEditVm orderEditVm)
    {
        _orderEditVm = orderEditVm;
        _category = category;

        Id = _category.Id;
        Name = category.Name;
        HasIcon = category.HasIcon;
        Image = category.Image;
        Color = category.Color;

        _disposable = modelManager.AddPresenter(this);
    }

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

        _category = category;
    }

    public override void Dispose() => _disposable.Dispose();
}
