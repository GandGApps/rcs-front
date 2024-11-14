using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using RcsInstaller.Vms;
using System;

namespace RcsInstaller;

public sealed partial class SelectPathPage : ReactiveUserControl<SelectPathPageVm>
{
    public SelectPathPage()
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

        if(result.Count == 0)
            return;

        ViewModel!.InstallPath = Uri.UnescapeDataString(result != null ? result[0].Path.AbsolutePath : string.Empty);
    }
}