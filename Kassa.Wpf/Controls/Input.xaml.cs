using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.BuisnessLogic;
using Kassa.RxUI;
using Kassa.RxUI.Dialogs;
using Splat;

namespace Kassa.Wpf.Controls;
/// <summary>
/// 
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

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label),
            typeof(UIElement),
            typeof(Input),
            new FrameworkPropertyMetadata(null)
        );

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(Input),
            new FrameworkPropertyMetadata(new CornerRadius(0))
        );

    private TextBlock? _placeholder;
    private TextBoxWithoutVirtualKeyboard? _input;

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public UIElement Label
    {
        get => (UIElement)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Input()
    {
        InitializeComponent();

    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();


        _placeholder = (TextBlock)GetTemplateChild("Placeholder");
        _input = (TextBoxWithoutVirtualKeyboard)GetTemplateChild("Input");

        if (string.IsNullOrWhiteSpace(Text))
        {
            _placeholder.Visibility = Visibility.Visible;
        }
        else
        {
            _placeholder.Visibility = Visibility.Collapsed;
        }

        _input.GotFocus += (_, _) =>
        {
            var name = (string?)GetValue(AutomationProperties.NameProperty);

            if (string.IsNullOrWhiteSpace(name))
            {
                name = Placeholder ?? string.Empty;
            }

            var inputDialog = new InputDialogViewModel(name, Text);

            inputDialog.OkCommand.Subscribe(x =>
            {
                Text = x;
            });

            var mainViewModel = Locator.Current.GetRequiredService<MainViewModel>();

            mainViewModel.DialogOpenCommand.Execute(inputDialog).Subscribe();
        };
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
