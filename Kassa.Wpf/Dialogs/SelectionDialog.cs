using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kassa.Wpf.Dialogs;
public static class SelectionDialog
{
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
        "HeaderTemplate", 
        typeof(DataTemplate), 
        typeof(SelectionDialog), 
        new PropertyMetadata(default(DataTemplate)));

    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.RegisterAttached(
        "ItemTemplate", 
        typeof(DataTemplate), 
        typeof(SelectionDialog), 
        new PropertyMetadata(default(DataTemplate)));

    public static DataTemplate GetHeaderTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(HeaderTemplateProperty);
    public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(HeaderTemplateProperty, value);

    public static DataTemplate GetItemTemplate(DependencyObject obj) => (DataTemplate)obj.GetValue(ItemTemplateProperty);
    public static void SetItemTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(ItemTemplateProperty, value);

}
