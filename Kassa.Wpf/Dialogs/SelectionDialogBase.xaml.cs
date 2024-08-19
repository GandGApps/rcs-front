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
using static Kassa.Wpf.Dialogs.WidePrimaryDialog;

namespace Kassa.Wpf.Dialogs;

public abstract class SelectionDialogBase<T> : ClosableDialog<T> where T : BaseSelectDialogViewModel
{  
    static SelectionDialogBase()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionDialogBase<T>), new FrameworkPropertyMetadata(typeof(SelectionDialogBase<T>)));
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
            this.OneWayBind(ViewModel, x => x.SearchText, x => x.SearchText)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsKeyboardVisible, x => x.IsKeyboardEnabled)
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

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == HeaderTemplateProperty)
        {
        }

        if (e.Property == SearchTextProperty)
        {
            ViewModel!.SearchText = (string)e.NewValue;
        }

        if (e.Property == IsKeyboardEnabledProperty)
        {
            ViewModel!.IsKeyboardVisible = (bool)e.NewValue;
        }
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
                // It might not be the best idea to use WeakEventManager here,
                // but I'm not sure if it will be garbage collected or not.
                WeakEventManager<UIElement, MouseButtonEventArgs>.AddHandler(clearIcon, nameof(MouseDown), (_, _) => ViewModel!.SearchText = string.Empty);
            }


        });

        


    }
}
