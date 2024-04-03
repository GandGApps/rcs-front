using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ProductView.xaml
/// </summary>
public partial class ProductView : ButtonUserControl<ProductDto>
{
    public ProductView() : base()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            var vm = new ProductViewModel(
                Splat.Locator.Current.GetNotInitializedService<IProductService>(),
                ViewModel!
            );

            DataContext = vm; 

            Command = ProductViewModel.AddToShoppingListCommand;
            CommandParameter = vm;

            if (ViewModel!.Icon is not null)
            {
                ViewModel.WhenAnyValue(x => x.Icon)
                    .Subscribe(x =>
                    {
                        var resource = Application.Current.TryFindResource(x);

                        if (resource is Geometry geometry)
                        {
                            ProductIcon.Data = geometry;
                        }
                        else
                        {
                            ProductIcon.Data = Geometry.Empty;
                        }
                    })
                    .DisposeWith(disposables);

            }
            else
            {
                ProductIcon.Data = Application.Current.TryFindResource("CupOfTeaIcon") as Geometry;
            }

            vm.DisposeWith(disposables);
        });
    }
}
