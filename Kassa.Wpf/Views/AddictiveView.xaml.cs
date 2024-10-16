﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
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
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Wpf.Views;
/// <summary>
/// Interaction logic for AddictiveView.xaml
/// </summary>
public partial class AddictiveView : ReactiveUserControl<AdditiveViewModel>
{

    public static readonly DependencyProperty HasAddictiveIconProperty = DependencyProperty.Register(
               nameof(HasAddictiveIcon), typeof(bool), typeof(AddictiveView), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
               nameof(Command), typeof(ICommand), typeof(AddictiveView), new PropertyMetadata(default(ICommand)));

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
               nameof(CommandParameter), typeof(object), typeof(AddictiveView), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty IsOnDialogProperty = DependencyProperty.Register(
               nameof(IsOnDialog), typeof(bool), typeof(AddictiveView), new PropertyMetadata(default(bool)));

    public bool HasAddictiveIcon
    {
        get => (bool)GetValue(HasAddictiveIconProperty);
        set => SetValue(HasAddictiveIconProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsOnDialog
    {
        get => (bool)GetValue(IsOnDialogProperty);
        set => SetValue(IsOnDialogProperty, value);
    }

    public AddictiveView()
    {
        InitializeComponent();



        this.WhenActivated(disposabels =>
        {
            Command = AdditiveViewModel.AddAdditveToProduct;
            CommandParameter = ViewModel!;

            this.WhenAnyValue(x => x.ViewModel)
                .Subscribe(x =>
                {
                    if (x is null)
                    {
                        DataContext = null;
                    }
                    else
                    {
                        DataContext = x;
                    }
                })
                .DisposeWith(disposabels);

            ViewModel!.DisposeWith(disposabels);
        });
    }
}
[EditorBrowsable(EditorBrowsableState.Never)]
public class DesignerAddictiveViewModel
{

    [Reactive]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Name
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        get; set;
    }

    [Reactive]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string СurrencySymbol
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        get; set;
    }

    [Reactive]
    public double Price
    {
        get; set;
    }

    [Reactive]
    public bool IsAdded
    {
        get; set;
    }

    [Reactive]
    public double Portion
    {
        get; set;
    }

    [Reactive]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Measure
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        get; set;
    }


    [Reactive]
    public bool IsAvailable
    {
        get; set;
    }

    [Reactive]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ReactiveCommand<Unit, Unit> AddToShoppingListCommand
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        get; set;
    }
}
