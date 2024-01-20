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
using Kassa.DataAccess;
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ProductView.xaml
/// </summary>
public partial class ProductView : ButtonUserControl<Product>
{
    public ProductView() : base()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = new ProductViewModel(ViewModel!);

            Command = ProductViewModel.AddToShoppingListCommand;
            CommandParameter = ViewModel;

            if (ViewModel!.Icon is not null)
            {
                var resource = Application.Current.TryFindResource(ViewModel!.Icon);

                if (resource is Geometry geometry)
                {
                    ProductIcon.Data = geometry;
                }

                return;
            }

            ProductIcon.Data = Application.Current.TryFindResource("CupOfTeaIcon") as Geometry;

        });
    }
}
