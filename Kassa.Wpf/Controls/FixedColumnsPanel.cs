using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using WpfToolkit.Controls;

namespace Kassa.Wpf.Controls;

public class FixedColumnsPanel : VirtualizingWrapPanel, IActivatableView
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(int), typeof(FixedColumnsPanel), new PropertyMetadata(default(int)));

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public FixedColumnsPanel()
    {
        this.WhenActivated(disposables =>
        {

            this.WhenAnyValue(x => x.ActualWidth, x => x.Columns, x => x.Children, x => x.Children.Count)
                .Subscribe(x =>
                {
                    var itemWidth = 0d;
                    if (x.Item1 > 0 && x.Item2 > 0)
                    {
                        itemWidth = x.Item1 / x.Item2;
                    }
                    var child = x.Item3.Cast<UIElement>().FirstOrDefault();
                    child?.Measure(new Size(itemWidth, double.PositiveInfinity));
                    var requiredHeight = child?.DesiredSize.Height ?? 0;

                    ItemSize = new Size(itemWidth, requiredHeight);
                })
                .DisposeWith(disposables);
        });
    }


}
