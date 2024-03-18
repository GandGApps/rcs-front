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
using ReactiveUI;
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
            typeof(object),
            typeof(Input),
            new FrameworkPropertyMetadata(null)
        );

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(Input),
            new FrameworkPropertyMetadata(new CornerRadius(0))
        );

    public static readonly DependencyProperty InputTypeProperty =
        DependencyProperty.Register(nameof(InputType),
        typeof(InputDialogType),
        typeof(Input),
        new FrameworkPropertyMetadata(InputDialogType.Text)
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

    public object Label
    {
        get => (object)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public InputDialogType InputType
    {
        get => (InputDialogType)GetValue(InputTypeProperty);
        set => SetValue(InputTypeProperty, value);
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

            var mainViewModel = Locator.Current.GetRequiredService<MainViewModel>();

            if (InputType is InputDialogType.Number or InputDialogType.Phone)
            {
                var inputDialog = new InputNumberDialogViewModel(name, Text);

                inputDialog.OkCommand.Subscribe(x =>
                {
                    Text = x;
                });

                if (InputType == InputDialogType.Phone)
                {
                    inputDialog.ClearCommand = ReactiveCommand.Create(() =>
                    {
                        inputDialog.Input = "+";
                    });

                    inputDialog.BackspaceCommand = ReactiveCommand.Create(() =>
                    {
                        if (inputDialog.Input?.Length > 1)
                        {
                            inputDialog.Input = inputDialog.Input[0..^1];
                        }
                        else
                        {
                            inputDialog.Input = "+";
                        }
                    });
                }
                else
                {
                    inputDialog.ClearCommand = ReactiveCommand.Create(() =>
                    {
                        inputDialog.Input = string.Empty;
                    });

                    inputDialog.BackspaceCommand = ReactiveCommand.Create(() =>
                    {

                        if (inputDialog.Input?.Length > 0)
                        {

                            inputDialog.Input = inputDialog.Input[0..^1];
                        }
                    });
                }

                mainViewModel.DialogOpenCommand.Execute(inputDialog).Subscribe();
            }
            else
            {
                var inputDialog = new InputDialogViewModel(name, Text);

                inputDialog.OkCommand.Subscribe(x =>
                {
                    Text = x;
                });

                mainViewModel.DialogOpenCommand.Execute(inputDialog).Subscribe();
            }


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
