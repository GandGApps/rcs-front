using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using Kassa.Wpf.Converters;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для ProductView.xaml
/// </summary>
public partial class ProductView : ButtonUserControl<ProductViewModel>
{
    private readonly BrushConverter _brushConverter = new();

    public ProductView() 
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

            this.OneWayBind(ViewModel, x => x.IsPriceVisible, x => x.PriceTextBlock.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Color, x => x.Background, x =>
            {
                var defaultBrush = (Brush)Resources["DefaultProductViewBackground"]; 

                if (x != string.Empty)
                {
                    return (Brush?)_brushConverter.ConvertFromString(x) ?? defaultBrush;
                }

                return (Brush)Resources["DefaultProductViewBackground"];

            }).DisposeWith(disposables);
        });

        Unloaded += (_, _) =>
        {
            Debug.Assert(ViewModel != null);

            ViewModel.Dispose();
        };
    }
}
