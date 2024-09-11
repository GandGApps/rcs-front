using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using Kassa.RxUI;
using Kassa.Wpf.MarkupExntesions;
using ReactiveUI;

namespace Kassa.Wpf.Controls;
/// <summary>
/// Interaction logic for Keyboard.xaml
/// </summary>
public partial class Keyboard : UserControl, IActivatableView
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(Keyboard)
    );

    public static readonly DependencyProperty KeyboardInfoProperty = DependencyProperty.Register(
        nameof(KeyboardInfo),
        typeof(KeyboardInfo),
        typeof(Keyboard)
    );

    public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register(
        nameof(EnterCommandProperty),
        typeof(ICommand),
        typeof(Keyboard)
    );

    public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register(
        nameof(TextBox),
        typeof(TextBox),
        typeof(Keyboard),
        new PropertyMetadata(null)
    );

    public static readonly DependencyProperty TextBoxesProperty = DependencyProperty.Register(
        nameof(TextBoxes),
        typeof(IEnumerable<TextBox>),
        typeof(Keyboard),
        new PropertyMetadata(Enumerable.Empty<TextBox>())
    );

    public TextBox? TextBox
    {
        get => (TextBox?)GetValue(TextBoxProperty);
        set => SetValue(TextBoxProperty, value);
    }

    public ICommand EnterCommand
    {
        get => (ICommand)GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public KeyboardInfo KeyboardInfo
    {
        get => (KeyboardInfo)GetValue(KeyboardInfoProperty);
        set => SetValue(KeyboardInfoProperty, value);
    }

    public IEnumerable<TextBox> TextBoxes
    {
        get => (IEnumerable<TextBox>)GetValue(TextBoxesProperty);
        set => SetValue(TextBoxesProperty, value);
    }

    private int CaretIndex
    {
        get => TextBox?.CaretIndex ?? Text?.Length ?? 0;
        set
        {
            if (TextBox is null)
            {
                return;
            }

            TextBox.CaretIndex = value;
        }
    }

    private int SelectionStart
    {

        get => TextBox?.SelectionStart ?? 0;
        set
        {

            if (TextBox is null)
            {
                return;
            }

            TextBox.SelectionStart = value;
        }
    }

    private int SelectionLength
    {

        get => TextBox?.SelectionLength ?? 0;
        set
        {

            if (TextBox is null)
            {

                return;
            }

            TextBox.SelectionLength = value;
        }
    }

    private static readonly KeySizeToGridWidthConverter _keySizeToWidthConverter = new();

    private int _textBoxesCount;
    private CompositeDisposable? _isFocusedEventDisposables;
    private CompositeDisposable? _currentTextBox;

    public Keyboard()
    {
        InitializeComponent();

        var changeToEnKeyInfo = new KeyInfo()
        {
            Text = "Ru",
        };
        var ruKeyboardInfo = KeyboardInfo.RuKeyboard(changeToEnKeyInfo);
        ruKeyboardInfo.PercentageKeyHeight = 0.60;

        var changeToRuKeyInfo = new KeyInfo()
        {
            Text = "En",
        };
        var enKeyboardInfo = KeyboardInfo.EnKeyboard(changeToRuKeyInfo);
        enKeyboardInfo.PercentageKeyHeight = 0.60;

        changeToEnKeyInfo.Command = ReactiveCommand.Create(() =>
        {
            KeyboardInfo = enKeyboardInfo;
        });

        changeToRuKeyInfo.Command = ReactiveCommand.Create(() =>
        {
            KeyboardInfo = ruKeyboardInfo;
        });

        KeyboardInfo = ruKeyboardInfo;

        Loaded += (s, e) =>
        {
            // Подписываемся на изменение размера родителя
            if (Parent is FrameworkElement parent && KeyboardInfo.PercentageKeyHeight.HasValue)
            {

                // Вычисляем 70% от высоты родителя
                var height = parent.ActualHeight * KeyboardInfo.PercentageKeyHeight.Value;

                // Устанавливаем высоту UserControl
                Height = height;

                parent.SizeChanged += (_, _) =>
                {
                    if (Parent is FrameworkElement parent && KeyboardInfo.PercentageKeyHeight.HasValue)
                    {
                        // Вычисляем 70% от высоты родителя
                        var height = parent.ActualHeight * KeyboardInfo.PercentageKeyHeight.Value;

                        // Устанавливаем высоту UserControl
                        Height = height;
                    }
                };
            }

        };

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.ActualWidth)
                .Subscribe(x => KeyboardInfo.LineWidth = x)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.KeyboardInfo, x => x.ActualWidth, x => x.Visibility)
                .Select(x => x.Item1)
                .Subscribe(x =>
                {
                    stack.Children.Clear();

                    foreach (var line in x.Lines)
                    {

                        var lineGrid = new Grid()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Focusable = false,
                        };

                        var lineDisposable = new CompositeDisposable();

                        lineGrid.Unloaded += (_, _) => lineDisposable.Dispose();

                        var size = 0d;
                        Thickness margin;

                        foreach (var key in line)
                        {
                            var button = new Button
                            {
                                DataContext = key,
                            };
                            button.SetResourceReference(StyleProperty, "KeyButtonStyle");

                            var keyDisposables = new CompositeDisposable();

                            key.WhenAnyValue(x => x.Character, x => x.Text, x => x.IsIcon, x => x.Icon, x => x.IsEnter, x => x.Command)
                                .Subscribe(x =>
                                {
                                    if (x.Item3 && !string.IsNullOrWhiteSpace(x.Item4))
                                    {
                                        var path = new Path();
                                        path.SetResourceReference(Path.DataProperty, x.Item4);
                                        path.SetResourceReference(Path.FillProperty, "KeyForeground");

                                        var iconAdaptiveSize = new AdaptiveSizeExtension(30);
                                        var binding = (BindingBase)iconAdaptiveSize.ProvideValue(null!);

                                        path.SetBinding(HeightProperty, binding);

                                        button.Content = path;
                                    }
                                    if (x.Item1 is not null && x.Item1 != ' ')
                                    {
                                        button.Content = x.Item1;
                                    }
                                    else if (x.Item2 is not null)
                                    {
                                        button.Content = x.Item2;
                                    }
                                    if (x.Item5 || x.Item6 is null)
                                    {
                                        key.Command = EnterCommand;
                                    }
                                    if (!string.IsNullOrWhiteSpace(x.Item2))
                                    {
                                        AdaptiveSizeExtension? size = null;

                                        if (x.Item2.Length > 4)
                                        {
                                            size = new(18);
                                        }

                                        if (x.Item2.Length > 7)
                                        {
                                            size = new(12);
                                        }

                                        if (size == null)
                                        {
                                            return;
                                        }

                                        var binding = (BindingBase)size.ProvideValue(null!);

                                        BindingOperations.ClearBinding(button, Button.FontSizeProperty);
                                        button.SetBinding(Button.FontSizeProperty, binding);
                                    }
                                });

                            key.PropertyChanged += (s, e) =>
                            {
                            };

                            button.Unloaded += (s, e) =>
                            {
                                keyDisposables.Dispose();
                            };

                            var columnsDefination = new ColumnDefinition
                            {
                                DataContext = key
                            };

                            columnsDefination.SetBinding(ColumnDefinition.WidthProperty, new Binding(nameof(KeyInfo.Size))
                            {
                                Converter = _keySizeToWidthConverter,
                            });

                            Grid.SetColumn(button, lineGrid.ColumnDefinitions.Count);

                            lineGrid.ColumnDefinitions.Add(columnsDefination);
                            lineGrid.Children.Add(button);
                            size += key.Size.Size;

                            margin = button.Margin;

                            if (key.Command is null)
                            {
                                if (key.IsBackspace)
                                {
                                    key.Command = ReactiveCommand.Create(() =>
                                    {
                                        RemoveLastCharacter();
                                    });
                                }
                                if (key.Character is not null)
                                {
                                    key.Command = ReactiveCommand.Create(() =>
                                    {
                                        AddCharacter(key.Character.Value);
                                    });
                                }
                                if (key.IsClear)
                                {
                                    key.Command = ReactiveCommand.Create(() =>
                                    {
                                        Text = string.Empty;
                                    });
                                }
                                if (key.IsRegister)
                                {
                                    key.Command = ReactiveCommand.Create(() =>
                                    {
                                        var allKeys = KeyboardInfo.Lines
                                                               .SelectMany(x => x);

                                        foreach (var keyInfo in allKeys)
                                        {
                                            if (keyInfo.Character is not char character)
                                            {
                                                continue;
                                            }
                                            if (!char.IsLetter(character))
                                            {
                                                continue;
                                            }
                                            if (char.IsUpper(character))
                                            {
                                                keyInfo.Character = char.ToLower(character);
                                            }
                                            else
                                            {
                                                keyInfo.Character = char.ToUpper(character);
                                            }
                                        }
                                    });
                                }
                            }

                            button.Command = key.Command;
                            button.CommandParameter = key.Parameter;

                            key.WhenAnyValue(x => x.Command, x => x.Parameter)
                               .Subscribe(x =>
                               {
                                   button.Command = x.Item1;
                                   button.CommandParameter = x.Item2;
                               })
                               .DisposeWith(keyDisposables);
                        }

                        var targetWidth = ActualWidth / x.LineStarWidth * (size);
                        Debug.WriteLine($"Target width for the keyboard {targetWidth}");

                        if (x.PercentageKeyHeight is not null)
                        {
                            this.WhenAnyValue(x => x.ActualHeight)
                                .Subscribe(_ =>
                                {
                                    lineGrid.Height = ActualHeight / x.Lines.Count;
                                })
                                .DisposeWith(lineDisposable);
                        }
                        else
                        {
                            var adaptiveHeight = new AdaptiveSizeExtension(x.KeyHeight + 4);
                            var bindingHeight = (BindingBase)adaptiveHeight.ProvideValue(null!);

                            lineGrid.SetBinding(HeightProperty, bindingHeight);
                        }

                        lineGrid.Width = targetWidth;


                        stack.Children.Add(lineGrid);
                    }
                })
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.TextBoxes)
                .Subscribe(x =>
                {
                    _isFocusedEventDisposables?.Dispose();
                    _isFocusedEventDisposables = [];

                    _textBoxesCount = x.Count();

                    foreach (var textBox in x)
                    {
                        textBox.WhenAnyValue(x => x.IsFocused)
                            .Where(x => x)
                            .Subscribe(x =>
                            {
                                TextBox = textBox;
                            })
                            .DisposeWith(_isFocusedEventDisposables);
                    }

                })
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.TextBox)
                .Subscribe(textBox =>
                {
                    _currentTextBox?.Dispose();

                    if (textBox is null)
                    {
                        return;
                    }

                    Text = textBox.Text;

                    // Two way binding

                    // Text -> TextBox
                    var disposable1 = this.WhenAnyValue(x => x.Text)
                        .Subscribe(x =>
                        {
                            x ??= string.Empty;

                            if (x != textBox.Text)
                            {
                                textBox.Text = x;
                            }
                        });

                    // TextBox -> Text
                    var disposable2 = textBox.WhenAnyValue(x => x.Text)
                        .Subscribe(x =>
                        {
                            x ??= string.Empty;

                            if (x != Text)
                            {
                                Text = x;
                            }
                        });

                    _currentTextBox = new CompositeDisposable(disposable1, disposable2);

                })
                .DisposeWith(disposables);
        });
    }
    private void RemoveLastCharacter()
    {
        if (SelectionLength > 0)
        {
            var tmp = SelectionLength;
            Text = Text.Remove(SelectionStart, SelectionLength);
            CaretIndex = tmp - 1;
            return;
        }

        if (CaretIndex > 0)
        {
            var tmp = CaretIndex;
            Text = Text.Remove(CaretIndex - 1, 1);
            CaretIndex = tmp - 1;
        }
    }

    private void AddCharacter(char character)
    {
        Text ??= string.Empty;

        if (SelectionLength > 0)
        {
            var tmp = SelectionStart;
            Text = Text.ReplaceAt(SelectionStart, SelectionLength, character.ToString());
            CaretIndex = tmp + 1;

            return;
        }

        var tmpCaretIndex = CaretIndex;
        Text = Text.Insert(CaretIndex, character.ToString());
        CaretIndex = tmpCaretIndex + 1;
    }

    private class KeySizeToGridWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var width = ((KeySize)value).Size;

            return new GridLength(width, GridUnitType.Star);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public static readonly KeyboardInfo Numpad = KeyboardInfo.IntegerNumpadWithDelivery();
}
