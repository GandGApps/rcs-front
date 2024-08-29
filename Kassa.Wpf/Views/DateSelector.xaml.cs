using System;
using System.Collections.Generic;
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
using Kassa.Shared;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for DateSelector.xaml
/// </summary>
public partial class DateSelector : UserControl, IActivatableView
{
    public static readonly DependencyProperty DateTimeProperty = DependencyProperty.Register(
        nameof(DateTime),
        typeof(DateTime),
        typeof(DateSelector)
    );

    public static readonly DependencyProperty LeftCommandProperty = DependencyProperty.Register(
        nameof(LeftCommand),
        typeof(ICommand),
        typeof(DateSelector)
    );

    public static readonly DependencyProperty LeftCommandParameterProperty = DependencyProperty.Register(
        nameof(LeftCommandParameter),
        typeof(object),
        typeof(DateSelector)
    );

    public static readonly DependencyProperty RightCommandProperty = DependencyProperty.Register(
        nameof(RightCommand),
        typeof(ICommand),
        typeof(DateSelector)
    );

    public static readonly DependencyProperty RightCommandParameterProperty = DependencyProperty.Register(
        nameof(RightCommandParameter),
        typeof(object),
        typeof(DateSelector)
    );

    public DateTime DateTime
    {
        get => (DateTime)GetValue(DateTimeProperty);
        set => SetValue(DateTimeProperty, value);
    }

    public ICommand LeftCommand
    {
        get => (ICommand)GetValue(LeftCommandProperty);
        set => SetValue(LeftCommandProperty, value);
    }

    public object? LeftCommandParameter
    {
        get => GetValue(LeftCommandParameterProperty);
        set => SetValue(LeftCommandParameterProperty, value);
    }

    public ICommand RightCommand
    {
        get => (ICommand)GetValue(RightCommandProperty);
        set => SetValue(RightCommandProperty, value);
    }

    public object? RightCommandParameter
    {
        get => GetValue(RightCommandParameterProperty);
        set => SetValue(RightCommandParameterProperty, value);
    }

    public DateSelector()
    {
        DateTime = DateTime.Today;

        LeftCommand = ReactiveCommand.Create(() =>
        {

            DateTime = DateTime.AddDays(-1);
        });

        RightCommand = ReactiveCommand.Create(() =>
        {
            DateTime = DateTime.AddDays(1);
        });


        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.DateTime)
                .Select(x => x.ToString("d MMMM yyyy", RcsKassa.RuCulture))
                .BindTo(this, x => x.CurrentDate.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.DateTime)
                .Select(x => x.Date == DateTime.Today ? "(Сегодня)" : string.Empty)
                .BindTo(this, x => x.RelativeInfo.Text)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.RightCommand)
                .BindTo(this, x => x.RightButton.Command)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.RightCommandParameter)
                .BindTo(this, x => x.RightButton.CommandParameter)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.LeftCommand)
                .BindTo(this, x => x.LeftButton.Command)
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.LeftCommandParameter)
                .BindTo(this, x => x.LeftButton.CommandParameter)
                .DisposeWith(disposables);
        });
    }
}
