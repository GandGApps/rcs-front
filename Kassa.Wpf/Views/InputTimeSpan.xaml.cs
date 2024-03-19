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

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for InputTimeSpan.xaml
/// </summary>
public partial class InputTimeSpan : UserControl
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(TimeSpan),
        typeof(InputTimeSpan),
        new PropertyMetadata(TimeSpan.Zero)
    );

    public static readonly DependencyProperty MinuteStepProperty = DependencyProperty.Register(
        nameof(MinuteStep),
        typeof(TimeSpan),
        typeof(InputTimeSpan),
        new PropertyMetadata(TimeSpan.FromMinutes(5))
    );

    public static readonly DependencyProperty HourStepProperty = DependencyProperty.Register(
        nameof(HourStep),
        typeof(TimeSpan),
        typeof(InputTimeSpan),
        new PropertyMetadata(TimeSpan.FromHours(1))
    );

    public TimeSpan Value
    {
        get => (TimeSpan)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public TimeSpan MinuteStep
    {
        get => (TimeSpan)GetValue(MinuteStepProperty);
        set => SetValue(MinuteStepProperty, value);
    }

    public TimeSpan HourStep
    {
        get 
        set;
    }

    public InputTimeSpan()
    {
        InitializeComponent();
    }
}
