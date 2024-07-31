using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using KassaLauncher.Vms;
using ReactiveUI;
using Tmds.DBus.Protocol;

namespace KassaLauncher;

public partial class InstallerView : ReactiveUserControl<InstallerVm>
{
    public InstallerView()
    {
        InitializeComponent();
    }

    private async void SelectPath(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;

        var result = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Выберите папку для установки"
        });

        ViewModel!.InstallPath = Uri.EscapeDataString(result != null ? result[0].Path.AbsolutePath : string.Empty);
    }
}