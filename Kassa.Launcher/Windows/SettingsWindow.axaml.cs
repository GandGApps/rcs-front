using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kassa.Launcher.Vms;
using SukiUI.Controls;

namespace Kassa.Launcher;

public partial class SettingsWindow : SukiWindow
{

    public static readonly DirectProperty<SettingsWindow, SettingsVm> ViewModelProperty = AvaloniaProperty.RegisterDirect<SettingsWindow, SettingsVm>(nameof(ViewModel), x => x.ViewModel);

    public SettingsVm ViewModel
    {
        get => (SettingsVm)DataContext!;
        private set
        {
            var oldValue = ViewModel;
            DataContext = value;

            RaisePropertyChanged(ViewModelProperty, oldValue, value);
        }
    }

    public SettingsWindow()
    {
        ViewModel = new SettingsVm();

        AvaloniaXamlLoader.Load(this);

        CanResize = false;
        CanMinimize = false;
        BackgroundAnimationEnabled = true;

        CanResize = false;
        MinWidth = Width;
        MinHeight = Height;
        MaxWidth = Width;
        MaxHeight = Height;

        ViewModel.SaveSettingsCommand.Subscribe(_ =>
        {
            Close();
        });
    }
}