using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]


[assembly:XmlnsDefinition("https://shema.kassa/wpf/controls",nameof(Kassa.Wpf.Controls))]
[assembly:XmlnsDefinition("https://shema.kassa/wpf/dialogs",nameof(Kassa.Wpf.Dialogs))]
[assembly:XmlnsDefinition("https://shema.kassa/wpf/views",nameof(Kassa.Wpf.Views))]
[assembly:XmlnsDefinition("https://shema.kassa/wpf/pages",nameof(Kassa.Wpf.Pages))]
[assembly:XmlnsDefinition("https://shema.kassa/rxui", nameof(Kassa.RxUI), AssemblyName = nameof(Kassa.RxUI))]
[assembly:XmlnsDefinition("https://shema.kassa/rxui/dialogs", nameof(Kassa.RxUI.Dialogs), AssemblyName = nameof(Kassa.RxUI))]
[assembly:XmlnsDefinition("https://shema.kassa/rxui/pages", nameof(Kassa.RxUI.Pages), AssemblyName = nameof(Kassa.RxUI.Pages))]