﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess;
using Kassa.RxUI;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Views;
/// <summary>
/// Логика взаимодействия для CategoryView.xaml
/// </summary>
public partial class CategoryView : ButtonUserControl<CategoryDto>
{
    public CategoryView()
    {
        InitializeComponent();

        Command = CategoryViewModel.MoveToCategoryCommand;
        this.WhenActivated(disposables =>
        {
            DataContext = new CategoryViewModel(ViewModel!);

            CommandParameter = ViewModel;
        });
    }
}
