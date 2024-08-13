using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kassa.RxUI.Dialogs;

namespace Kassa.Wpf.Dialogs;

public abstract class SelectionDialogBase<T> : ClosableDialog<T> where T : BaseSelectDialogViewModel
{
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
        nameof(HeaderTemplate), typeof(DataTemplate), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(DataTemplate)));

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(DataTemplate)));

    public static readonly DependencyProperty IsKeyboardVisibleProperty = DependencyProperty.Register(
        nameof(IsKeyboardVisible), typeof(bool), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty IsKeyboardEnabledProperty = DependencyProperty.Register(
        nameof(IsKeyboardEnabled), typeof(bool), typeof(SelectionDialogBase<T>), new PropertyMetadata(default(bool)));

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


    public DataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
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

    public SelectionDialogBase()
    {
        ClearValue(HeaderTemplateProperty);
    }

}
