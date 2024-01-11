﻿using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using ReactiveUI;
using Splat;
using Kassa.BuisnessLogic;

namespace Kassa.Wpf;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    public App()
    {
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterBuisnessLogic();
    }
}

