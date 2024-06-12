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
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using Kassa.Wpf.Converters;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для CategoryView.xaml
/// </summary>
public partial class CategoryView : ButtonUserControl<CategoryViewModel>
{
    private static readonly BrushConverter _brushConverter = new();

    public CategoryView()
    {
        InitializeComponent();

        Command = CategoryViewModel.MoveToCategoryCommand;

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel != null);

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

            this.OneWayBind(ViewModel, x => x.Color, x => x.Background, x =>
            {
                var defaultBrush = (Brush)App.Current.Resources["DefaultProductViewBackground"];

                if (!string.IsNullOrWhiteSpace(x))
                {
                    return (Brush?)_brushConverter.ConvertFromString(x) ?? defaultBrush;
                }

                return defaultBrush;

            }).DisposeWith(disposables);
        });
    }
}
