using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kassa.Wpf.Dialogs;
public static class WidePrimaryDialog
{
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
        "HeaderTemplate", 
        typeof(DataTemplate), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(DataTemplate)));

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached(
        "ItemTemplate", 
        typeof(DataTemplate), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(DataTemplate)));

    // TODO: Replace with IsKeyboardEnabled
    public static readonly DependencyProperty IsKeyboardVisibleProperty = DependencyProperty.RegisterAttached(
        "IsKeyboardVisible", 
        typeof(bool), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(bool), OnIsKeyboardVisibleChanged));

    // TODO: Replace with IsKeyboardVisible
    public static readonly DependencyProperty IsKeyboardEnabledProperty = DependencyProperty.RegisterAttached(
        "IsKeyboardEnabled", 
        typeof(bool), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.RegisterAttached(
        "SearchText", 
        typeof(string), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(string)));

    public static readonly DependencyProperty KeyboardVisibilityTextProperty = DependencyProperty.RegisterAttached(
        "KeyboardVisibilityText", 
        typeof(string), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached(
        "ItemsSource", 
        typeof(IEnumerable), 
        typeof(WidePrimaryDialog), 
        new PropertyMetadata(default(IEnumerable)));

    private static void OnIsKeyboardVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DependencyObject dialog)
        {
            var value = (bool)e.NewValue;

            if (!value)
            {
                dialog.SetValue(IsKeyboardEnabledProperty, false);
            }
        }
    }

    public static DataTemplate GetHeaderTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(HeaderTemplateProperty);
    public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(HeaderTemplateProperty, value);

    public static DataTemplate GetItemTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(ItemTemplateProperty);
    public static void SetItemTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(ItemTemplateProperty, value);

    public static bool GetIsKeyboardVisible(DependencyObject obj) => (bool)obj.GetValue(IsKeyboardVisibleProperty);
    public static void SetIsKeyboardVisible(DependencyObject obj, bool value) => obj.SetValue(IsKeyboardVisibleProperty, value);

    public static bool GetIsKeyboardEnabled(DependencyObject obj) => (bool)obj.GetValue(IsKeyboardEnabledProperty);
    public static void SetIsKeyboardEnabled(DependencyObject obj, bool value) => obj.SetValue(IsKeyboardEnabledProperty, value);

    public static string GetSearchText(DependencyObject obj) => (string)obj.GetValue(SearchTextProperty);
    public static void SetSearchText(DependencyObject obj, string value) => obj.SetValue(SearchTextProperty, value);

    public static string GetKeyboardVisibilityText(DependencyObject obj) => (string)obj.GetValue(KeyboardVisibilityTextProperty);
    public static void SetKeyboardVisibilityText(DependencyObject obj, string value) => obj.SetValue(KeyboardVisibilityTextProperty, value);

    public static IEnumerable GetItemsSource(DependencyObject obj) => (IEnumerable)obj.GetValue(ItemsSourceProperty);
    public static void SetItemsSource(DependencyObject obj, IEnumerable value) => obj.SetValue(ItemsSourceProperty, value);


}
