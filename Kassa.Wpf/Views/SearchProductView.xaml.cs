using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Kassa.Wpf.Converters;
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
            Debug.Assert(ViewModel != null);

            Command = ProductViewModel.AddToShoppingListCommand;
            CommandParameter = ViewModel;

            if (ViewModel.Image >= 0)
            {
                ViewModel.WhenAnyValue(x => x.Image)
                    .Subscribe(x =>
                    {
                        var resource = IntToProjectIcon.GetProjectIcon(x);

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
                // TODO: Load image from internet
                ProductIcon.Data = Application.Current.TryFindResource("CupOfTeaIcon") as Geometry;
            }

            ViewModel.DisposeWith(disposables);
        });
    }
}
