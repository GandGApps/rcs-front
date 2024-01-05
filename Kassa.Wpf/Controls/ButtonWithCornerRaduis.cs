using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kassa.Wpf.Controls;
public class ButtonWithCornerRaduis : Button
{
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(ButtonWithCornerRaduis),
        new PropertyMetadata(new CornerRadius(0))
    );

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    static ButtonWithCornerRaduis()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonWithCornerRaduis),
            new FrameworkPropertyMetadata(typeof(ButtonWithCornerRaduis)));
    }
}
