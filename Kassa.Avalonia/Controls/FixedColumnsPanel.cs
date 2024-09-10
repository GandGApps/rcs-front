using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Kassa.Avalonia.Controls;
public sealed class FixedColumnsPanel : WrapPanel
{

    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<FixedColumnsPanel, int>(nameof(Columns), defaultValue: 0);

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="availableSize">not returns positive inifinite</param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);

        var children = Children;
        var count = children.Count;
        var rows = count / Columns + (count % Columns > 0 ? 1 : 0);
        var width = availableSize.Width / Columns;
        var height = 80d;
        foreach (Layoutable child in children)
        {
            var size = new Size(width, availableSize.Height);
            child.Measure(size);
            height = child.DesiredSize.Height;
        }

        return new(availableSize.Width, height * rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        base.ArrangeOverride(finalSize);

        var children = Children;
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
