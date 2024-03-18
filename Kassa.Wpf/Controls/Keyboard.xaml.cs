using System;
using System.Collections.Generic;
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

    public ICommand EnterCommand
    {
        get => (ICommand)GetValue(EnterCommandProperty);
        set => SetValue(EnterCommandProperty, value);
    }

    private static readonly KeySizeToGridWidthConverter _keySizeToWidthConverter = new();

    public Keyboard()
    {
        InitializeComponent();

        var changeToEnKeyInfo = new KeyInfo()
        {
            Text = "Ru",
        };
        var ruKeyboardInfo = KeyboardInfo.RuKeyboard(changeToEnKeyInfo);

        var changeToRuKeyInfo = new KeyInfo()
        {
            Text = "En",
        };
        var enKeyboardInfo = KeyboardInfo.EnKeyboard(changeToRuKeyInfo);

        changeToEnKeyInfo.Command = ReactiveCommand.Create(() =>
        {
            KeyboardInfo = enKeyboardInfo;
        });

        changeToRuKeyInfo.Command = ReactiveCommand.Create(() =>
        {
            KeyboardInfo = ruKeyboardInfo;
        });

        KeyboardInfo = ruKeyboardInfo;

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

                                        if (size == null) return;

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

                                        if (Text?.Length > 0)
                                        {

                                            Text = Text.Remove(Text.Length - 1);
                                        }
                                    });
                                }
                                if (key.Character is not null)
                                {
                                    key.Command = ReactiveCommand.Create(() =>
                                    {
                                        Text += key.Character;
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

                        var adaptiveWidth = new AdaptiveSizeExtension((AdaptiveMarkupExtension.GetNotAdaptivedSize(ActualWidth, MainWindow.Instance.ActualWidth) / x.LineStarWidth) * (size));
                        var bindingWidth = (BindingBase)adaptiveWidth.ProvideValue(null!);

                        var adaptiveHeight = new AdaptiveSizeExtension(x.KeyHeight + 4);
                        var bindingHeight = (BindingBase)adaptiveHeight.ProvideValue(null!);

                        lineGrid.SetBinding(WidthProperty, bindingWidth);
                        lineGrid.SetBinding(HeightProperty, bindingHeight);

                        stack.Children.Add(lineGrid);
                    }
                })
                .DisposeWith(disposables);
        });
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
