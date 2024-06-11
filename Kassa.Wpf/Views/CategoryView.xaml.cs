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
    }
}
