using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Kassa.Wpf.Controls;

public partial class StatusEllipsedButton : ButtonWithCornerRaduis
{
    public static readonly DependencyProperty EllipseTextProperty = DependencyProperty.Register(
        nameof(EllipseText),
        typeof(string),
        typeof(StatusEllipsedButton),
        new PropertyMetadata(string.Empty)
    );

    public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
        nameof(StatusText),
        typeof(string),
        typeof(StatusEllipsedButton),
        new PropertyMetadata(string.Empty)
    );

    public string EllipseText
    {
        get => (string)GetValue(EllipseTextProperty);
        set => SetValue(EllipseTextProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }


    public StatusEllipsedButton()
    {
        InitializeComponent();
    }
}
