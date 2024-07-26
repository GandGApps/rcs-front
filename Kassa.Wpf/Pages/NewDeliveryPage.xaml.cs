using System;
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
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Interaction logic for NewDeliveryPage.xaml
/// </summary>
public partial class NewDeliveryPage : ReactiveUserControl<NewDeliveryPageVm>
{
    public NewDeliveryPage()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            Debug.Assert(ViewModel is not null);

            this.Bind(ViewModel, vm => vm.FirstName, v => v.FirstName.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.MiddleName, v => v.MiddleName.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.LastName, v => v.LastName.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Phone, v => v.Phone.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Card, v => v.Card.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Miscellaneous, v => v.Miscellaneous.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Comment, v => v.Comment.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.House, v => v.House.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Building, v => v.Building.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Entrance, v => v.Entrance.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Floor, v => v.Floor.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Apartment, v => v.Apartment.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.Intercom, v => v.Intercom.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.TypeOfOrder, v => v.TypeOfOrder.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.AddressNote, v => v.AddressNote.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.CourierFullName, v => v.Courier.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SelectDistrictAndStreetCommand, v => v.SelectDistrict)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SelectDistrictAndStreetCommand, v => v.SelectStreet)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SelectCourierCommand, v => v.Courier)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.District, v => v.DistrictName.Text, d => d is null || string.IsNullOrWhiteSpace(d.Name) ? "Не задан" : d.Name)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Street, v => v.StreetName.Text, s => s is null || string.IsNullOrWhiteSpace(s.Name) ? "Не задана" : s.Name)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.OrderEditPageVm.ShoppingList.Total, v => v.Price.Text, x => $"{x.ToString("0.##", QuantityVolumeDialogVewModel.RuCultureInfo)} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderEditPageVm.ShoppingList.ProductShoppingListItems, x => x.ShoppingListItems.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsAddressInfoVisible, x => x.BasicAddressInfo.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsAddressInfoVisible, x => x.AdditionalAddressInfo.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsAddressInfoVisible, x => x.AddressNote.Visibility)
                .DisposeWith(disposables);
            
            this.OneWayBind(ViewModel, x => x.IsDelivery, x => x.Courier.Visibility)
                .DisposeWith(disposables);

            OrderEditPage.ViewModel = ViewModel.OrderEditPageVm;
            PaymentPage.ViewModel = ViewModel.PaymentPageVm;

            BackButton.Command = ViewModel.BackButtonCommand;
            Problem.Command = ViewModel.WriteProblemCommand;
            Save.Command = ViewModel.SaveCommand;
        });
    }
}
