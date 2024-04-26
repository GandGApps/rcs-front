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
/// Interaction logic for SearchProductView.xaml
/// </summary>
public partial class SearchProductView : ButtonUserControl<ProductViewModel>
{
    public SearchProductView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            Command = ProductViewModel.AddToShoppingListCommand;
            CommandParameter = ViewModel;

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

            ViewModel.DisposeWith(disposables);
        });
    }
}
