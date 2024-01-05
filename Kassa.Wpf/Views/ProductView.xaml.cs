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
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ProductView.xaml
/// </summary>
public partial class ProductView : ButtonUserControl<ProductViewModel>
{
    public ProductView() : base()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            ObserveDataContext(disposables);
            ObserveViewModel(disposables);


            ViewModel.WhenAnyValue(x => x.Icon)
                     .Subscribe(icon =>
                     {
                         if (icon is not null)
                         {
                             var resource = Application.Current.TryFindResource(icon);

                             if (resource is Geometry geometry)
                             {
                                 ProductIcon.Data = geometry;
                             }

                             return;
                         }

                         ProductIcon.Data = Application.Current.TryFindResource("CupOfTeaIcon") as Geometry;
                     })
                     .DisposeWith(disposables);
        });
    }
}
