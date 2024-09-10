using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI;
using Kassa.Shared.ServiceLocator;
using ReactiveUI;

namespace Kassa.Avalonia.Controls;
public sealed class Input: Button
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<Input, string?>(nameof(Text), string.Empty);

    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<Input, string>(nameof(Placeholder), string.Empty);

    public static readonly StyledProperty<object?> LabelProperty =
        AvaloniaProperty.Register<Input, object?>(nameof(Label), null);

    public static readonly StyledProperty<InputDialogType> InputTypeProperty =
        AvaloniaProperty.Register<Input, InputDialogType>(nameof(InputType), InputDialogType.Text);

    static Input()
    {
        TextProperty.Changed.AddClassHandler<Input, string?>(TextChangedCallback);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public object? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }


    public InputDialogType InputType
    {
        get => GetValue(InputTypeProperty);
        set => SetValue(InputTypeProperty, value);
    }

    private TextBlock? _placeholder;
    private TextBoxWithoutVirtualKeyboard? _input;


    public Input()
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);


        _placeholder = e.NameScope.Get<TextBlock>("Placeholder");
        _input = e.NameScope.Get<TextBoxWithoutVirtualKeyboard>("Input");

        if (string.IsNullOrWhiteSpace(Text))
        {
            _placeholder.IsVisible = true;
        }
        else
        {
            _placeholder.IsVisible = false;
        }

        _input.GotFocus += (_, _) =>
        {
            var name = (string?)GetValue(AutomationProperties.NameProperty);

            if (string.IsNullOrWhiteSpace(name))
            {
                name = Placeholder ?? string.Empty;
            }

            var mainViewModel = RcsLocator.GetRequiredService<MainViewModel>();

            if (Command is not null)
            {
                Command.Execute(CommandParameter);
                return;
            }

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

    public static void TextChangedCallback(Input input, AvaloniaPropertyChangedEventArgs<string?> e)
    {
        if (input._placeholder == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(input.Text))
        {

            input._placeholder.IsVisible = true;
        }
        else
        {
            input._placeholder.IsVisible = false;
        }
    }
}
