using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Kassa.Avalonia.Pages;

public sealed partial class AutorizationPage : UserControl
{
    public AutorizationPage()
    {
        InitializeComponent();
    }

    private void NextClicked(object? sender, RoutedEventArgs e)
    {
        Welcome.Classes.Add("NextState");
        Logo.Classes.Add("NextState");
        Form.Classes.Add("NextState");
    }
}