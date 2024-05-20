using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using DynamicData;
using ReactiveUI;

namespace Kassa.Wpf.Windows;
/// <summary>
/// Interaction logic for DeveloperWindow.xaml
/// </summary>
public partial class DeveloperWindow : Window, IActivatableView
{
    private readonly ObservableCollection<string> _messages = [];

    private static DeveloperWindow? _instance;

    public static DeveloperWindow? Instance => _instance;

    public DeveloperWindow(IObservable<string> observable)
    {
        _instance ??= this;

        InitializeComponent();

        MessageListBox.ItemsSource = _messages;

        this.WhenActivated(disposables =>
        {
            observable.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x =>
            {
                _messages.Add(x);
            }).DisposeWith(disposables);
        });

        Closed += (sender, e) => _instance = null;
    }

    private void AddMessageButton_Click(object sender, RoutedEventArgs e)
    {
        var message = MessageTextBox.Text;
        if (!string.IsNullOrWhiteSpace(message))
        {
            _messages.Add(message);
            MessageTextBox.Clear();
        }
    }

    public void AddMessage(string message)
    {
        _messages.Add(message);
    }

    private void ClearMessagesButton_Click(object sender, RoutedEventArgs e)
    {
        _messages.Clear();
    }
}
