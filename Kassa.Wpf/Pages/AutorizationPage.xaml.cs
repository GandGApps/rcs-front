﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Kassa.RxUI.Pages;
using Kassa.Wpf.Controls;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Логика взаимодействия для AutorizationPage.xaml
/// </summary>
public partial class AutorizationPage : ReactiveUserControl<AutorizationPageVm>
{
    public static readonly RoutedEvent NextClickedEvent = EventManager.RegisterRoutedEvent("NextClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutorizationPage));

    public event RoutedEventHandler NextClicked
    {

        add => AddHandler(NextClickedEvent, value);
        remove => RemoveHandler(NextClickedEvent, value);
    }

    private IDisposable? _keyboardBinding;
    private object _keyboardTarget;

    private IDisposable? KeyboardBinding
    {
        get => _keyboardBinding;
        set
        {
            _keyboardBinding?.Dispose();
            _keyboardBinding = value;
        }
    }

    public AutorizationPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.BindCommand(ViewModel, x => x.CloseCommand, x => x.CloseButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Login, x => x.Login.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, x => x.Password, x => x.Password.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.LoginCommand, x => x.Submit)
                .DisposeWith(disposables);
        });

        AddTextBoxBehavior(Login, "Типа логин");
        AddTextBoxBehavior(Password, "Типа пароль");

        Keyboard.EnterCommand = ReactiveCommand.Create(() =>
        {
            Keyboard.Visibility = Visibility.Collapsed;
        });
    }

    private void AddTextBoxBehavior(TextBox textBox, string placeHolder)
    {
        textBox.GotFocus += (_, _) =>
        {
            RemoveText(textBox, placeHolder);

            Keyboard.Visibility = Visibility.Visible;
            Keyboard.Text = textBox.Text;

            var binding = new Binding
            {
                Source = textBox,
                Path = new(TextBox.TextProperty),
                Mode = BindingMode.TwoWay,
            };

            Keyboard.SetBinding(Controls.Keyboard.TextProperty, binding);

            KeyboardBinding = Disposable.Create(() =>
            {
                BindingOperations.ClearBinding(Keyboard, Controls.Keyboard.TextProperty);
            });

            _keyboardTarget = textBox;

        };
        textBox.LostFocus += (_, _) =>
        {
            var focused = System.Windows.Input.Keyboard.FocusedElement;

            if (focused is Button)
            {
                return;
            }
            AddText(textBox, placeHolder);

            if (_keyboardTarget == textBox)
            {
                KeyboardBinding = null;
                Keyboard.Visibility = Visibility.Collapsed;
            }
        };


    }

    private void AddText(TextBox sender, string placeholder)
    {
        if (string.IsNullOrEmpty(sender.Text))
        {
            sender.Text = placeholder;
        }
    }

    private void RemoveText(TextBox sender, string placeholder)
    {
        if (sender.Text == placeholder)
        {
            sender.Text = "";
        }
    }

    private void LogoSizeAnimationCompleted(object sender, EventArgs e)
    {
        Form.IsHitTestVisible = true;
        Welcome.IsHitTestVisible = false;
    }

}
