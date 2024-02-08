using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
/// <summary>
/// Логика взаимодействия для Input.xaml
/// </summary>
public partial class Input : UserControl
{

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text),
            typeof(string),
            typeof(Input),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, propertyChangedCallback: TextChangedCallback)
        );

    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(nameof(Placeholder),
            typeof(string),
            typeof(Input),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    private TextBlock? _placeholder;

    public Input()
    {
        InitializeComponent();


    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _placeholder = (TextBlock)GetTemplateChild("Placeholder");
    }

    public static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var input = (Input)d;

        if (input._placeholder == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(input.Text))
        {

            input._placeholder.Visibility = Visibility.Visible;
        }
        else
        {
            input._placeholder.Visibility = Visibility.Collapsed;
        }
    }
}
