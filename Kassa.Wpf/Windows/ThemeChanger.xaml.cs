using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ColorPicker;

namespace Kassa.Wpf.Windows;
/// <summary>
/// Логика взаимодействия для ThemeChanger.xaml
/// </summary>
public partial class ThemeChanger : Window
{
    public ThemeChanger()
    {
        InitializeComponent();
    }

    private void PortableColorPicker_ColorChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PortableColorPicker portableColorPicker)
        {
            return;
        }

        if (App.Current.Resources[portableColorPicker.Tag].Equals(portableColorPicker.SelectedColor))
        {
            return;
        }

        App.SetSolidBrush($"{portableColorPicker.Tag}", portableColorPicker.SelectedColor);
    }
}
