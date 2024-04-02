using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess;
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
