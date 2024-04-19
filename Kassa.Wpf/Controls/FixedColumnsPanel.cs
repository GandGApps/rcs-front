using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kassa.Wpf.Controls;

public class FixedColumnsPanel : WrapPanel
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(int), typeof(FixedColumnsPanel), new PropertyMetadata(default(int)));

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="availableSize">not returns positive inifinite</param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        var children = InternalChildren;
        var count = children.Count;
        var rows = count / Columns + (count % Columns > 0 ? 1 : 0);
        var width = availableSize.Width / Columns;
        var height = 80d;
        foreach (UIElement child in children)
        {
            var size = new Size(width, availableSize.Height);
            child.Measure(size);
            height = child.DesiredSize.Height;
        }
        return new(availableSize.Width, height*rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        base.ArrangeOverride(finalSize);
        var children = InternalChildren;
        var count = children.Count;
        /*var rows = count / Columns + (count % Columns > 0 ? 1 : 0);*/
        var width = finalSize.Width / Columns;
        /*var height = finalSize.Height / rows;*/
        for (var i = 0; i < count; i++)
        {
            var child = children[i];
            var height = child.DesiredSize.Height;
            var row = i / Columns;
            var column = i % Columns;
            var rect = new Rect(column * width, row * height, width, height);
            child.Arrange(rect);
        }
        return finalSize;
    }


}
