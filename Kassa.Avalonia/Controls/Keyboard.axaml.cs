using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kassa.RxUI;

namespace Kassa.Avalonia;

public sealed partial class Keyboard : UserControl
{
    public static readonly StyledProperty<string> TextProperty = 
        AvaloniaProperty.Register<Keyboard, string>(nameof(Text));

    public static readonly StyledProperty<KeyboardInfo> KeyboardInfoProperty =
        AvaloniaProperty.Register<Keyboard, KeyboardInfo>(nameof(KeyboardInfo));

    public static readonly StyledProperty<ICommand> EnterCommandProperty =
    AvaloniaProperty.Register<Keyboard, ICommand>(nameof(EnterCommand));

    public static readonly StyledProperty<TextBox> TextBoxProperty =
        AvaloniaProperty.Register<Keyboard, TextBox>(nameof(TextBox));

    public static readonly StyledProperty<IEnumerable<TextBox>> TextBoxesProperty =
        AvaloniaProperty.Register<Keyboard, IEnumerable<TextBox>>(nameof(TextBoxes), defaultValue: []);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public KeyboardInfo KeyboardInfo
    {
        get => GetValue(KeyboardInfoProperty);
        set => SetValue(KeyboardInfoProperty, value);
    }

    public ICommand EnterCommand
    {
        get => GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    public TextBox TextBox
    {
        get => GetValue(TextBoxProperty);
        set => SetValue(TextBoxProperty, value);
    }

    public IEnumerable<TextBox> TextBoxes
    {
        get => GetValue(TextBoxesProperty);
        set => SetValue(TextBoxesProperty, value);
    }

    public Keyboard()
    {
        InitializeComponent();
    }
}