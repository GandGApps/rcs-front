using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using ReactiveUI;
using Splat;
using Kassa.BuisnessLogic;
using Kassa.DataAccess;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static void SetSolidBrush(string brushKey, Color color)
    {
        var app = (App)Current;
        var merged = app.Resources.MergedDictionaries[0];
        merged[brushKey] = new SolidColorBrush(color);
    }
    public App()
    {
        InitializeComponent();

        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterMockDataAccess(); // TODO: Replace with real data access
        Locator.CurrentMutable.RegisterBuisnessLogic();
    }
}

