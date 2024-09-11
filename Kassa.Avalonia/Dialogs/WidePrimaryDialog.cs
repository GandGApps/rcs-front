using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;

namespace Kassa.Avalonia.Dialogs;
public class WidePrimaryDialog
{
    public static readonly AttachedProperty<Control> HeaderProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, Control>("Header");

    public static readonly AttachedProperty<DataTemplate> ItemTemplateProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, DataTemplate>("ItemTemplate");

    public static readonly AttachedProperty<bool> IsKeyboardVisibleProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, bool>("IsKeyboardVisible");

    public static readonly AttachedProperty<bool> IsKeyboardEnabledProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, bool>("IsKeyboardEnabled");

    public static readonly AttachedProperty<string> SearchTextProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, string>("SearchText");

    public static readonly AttachedProperty<string> KeyboardVisibilityTextProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, string>("KeyboardVisibilityText");

    public static readonly AttachedProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, IEnumerable>("ItemsSource");

    public static readonly AttachedProperty<Control> FooterProperty =
        AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, Control>("Footer");

    public static readonly AttachedProperty<TextBox> CurrentTextBoxProperty
        = AvaloniaProperty.RegisterAttached<WidePrimaryDialog, Control, TextBox>("CurrentTextBox");

    static WidePrimaryDialog()
    {
        IsKeyboardEnabledProperty.Changed.Subscribe(x =>
        {
            var value = x.NewValue.Value;

            if (x.Sender is Control control)
            {
                if (x.NewValue.Value == false)
                {
                    control.SetValue(IsKeyboardVisibleProperty, false);
                }
            }
        });
    }

    public static Control GetHeader(Control control)
    {
        return control.GetValue(HeaderProperty);
    }

    public static void SetHeader(Control control, Control value)
    {
        control.SetValue(HeaderProperty, value);
    }

    public static DataTemplate GetItemTemplate(Control control)
    {
        return control.GetValue(ItemTemplateProperty);
    }

    public static void SetItemTemplate(Control control, DataTemplate value)
    {
        control.SetValue(ItemTemplateProperty, value);
    }

    public static bool GetIsKeyboardVisible(Control control)
    {
        return control.GetValue(IsKeyboardVisibleProperty);
    }

    public static void SetIsKeyboardVisible(Control control, bool value)
    {
        control.SetValue(IsKeyboardVisibleProperty, value);
    }

    public static bool GetIsKeyboardEnabled(Control control)
    {
        return control.GetValue(IsKeyboardEnabledProperty);
    }

    public static void SetIsKeyboardEnabled(Control control, bool value)
    {
        control.SetValue(IsKeyboardEnabledProperty, value);
    }

    public static string GetSearchText(Control control)
    {
        return control.GetValue(SearchTextProperty);
    }

    public static void SetSearchText(Control control, string value)
    {
        control.SetValue(SearchTextProperty, value);
    }

    public static string GetKeyboardVisibilityText(Control control)
    {
        return control.GetValue(KeyboardVisibilityTextProperty);
    }

    public static void SetKeyboardVisibilityText(Control control, string value)
    {
        control.SetValue(KeyboardVisibilityTextProperty, value);
    }

    public static IEnumerable GetItemsSource(Control control)
    {
        return control.GetValue(ItemsSourceProperty);
    }

    public static void SetItemsSource(Control control, IEnumerable value)
    {
        control.SetValue(ItemsSourceProperty, value);
    }

    public static Control GetFooter(Control control)
    {
        return control.GetValue(FooterProperty);
    }

    public static void SetFooter(Control control, Control value)
    {
        control.SetValue(FooterProperty, value);
    }

    public static TextBox GetCurrentTextBox(Control control)
    {
        return control.GetValue(CurrentTextBoxProperty);
    }

    public static void SetCurrentTextBox(Control control, TextBox value)
    {
        control.SetValue(CurrentTextBoxProperty, value);
    }

}
