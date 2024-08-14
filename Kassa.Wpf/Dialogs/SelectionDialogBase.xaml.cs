using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;

public abstract class SelectionDialogBase<T> : ClosableDialog<T> where T : BaseSelectDialogViewModel
{

    // TODO: Replace with IsKeyboardEnabled
    public static readonly DependencyProperty IsKeyboardVisibleProperty = DependencyProperty.Register(
        nameof(IsKeyboardVisible), typeof(bool), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(bool), OnIsKeyboardVisibleChanged));

    // TODO: Replace with IsKeyboardVisible
    public static readonly DependencyProperty IsKeyboardEnabledProperty = DependencyProperty.Register(
        nameof(IsKeyboardEnabled), typeof(bool), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
        nameof(SearchText), typeof(string), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty KeyboardVisibilityTextProperty = DependencyProperty.Register(
        nameof(KeyboardVisibilityText), typeof(string), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(IEnumerable), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(IEnumerable)));

    static SelectionDialogBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionDialogBase<T>), new FrameworkPropertyMetadata(typeof(SelectionDialogBase<T>)));
    }

    private static void OnIsKeyboardVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelectionDialogBase<T> dialog)
        {
            var value = (bool)e.NewValue;

            if (!value)
            {
                dialog.IsKeyboardEnabled = false;
            }
        }
    }

    public bool IsKeyboardVisible
    {
        get => (bool)GetValue(IsKeyboardVisibleProperty);
        set => SetValue(IsKeyboardVisibleProperty, value);
    }

    public bool IsKeyboardEnabled
    {
        get => (bool)GetValue(IsKeyboardEnabledProperty);
        set => SetValue(IsKeyboardEnabledProperty, value);
    }

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public string KeyboardVisibilityText
    {
        get => (string)GetValue(KeyboardVisibilityTextProperty);
        set => SetValue(KeyboardVisibilityTextProperty, value);
    }

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public SelectionDialogBase()
    {
        ClearValue(TemplateProperty);

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, x => x.SearchText, x => x.SearchText)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.IsKeyboardVisible, x => x.IsKeyboardEnabled)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.Items, x => x.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                vm => vm.IsKeyboardVisible,
                v => v.KeyboardVisibilityText,
                x => x ? "Вкл" : "Выкл"
            ).DisposeWith(disposables);
        });
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel != null);

            if (GetTemplateChild("CancelButton") is Button cancelButton)
            {
                cancelButton.Command = ViewModel!.CloseCommand;
            }


            if (GetTemplateChild("ClearIcon") is UIElement clearIcon)
            {
                WeakEventManager<UIElement, MouseButtonEventArgs>.AddHandler(clearIcon, nameof(MouseDown), (_, _) => ViewModel!.SearchText = string.Empty);
            }


        });

        


    }
}
