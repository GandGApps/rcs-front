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
using Kassa.DataAccess.Model;
using Kassa.RxUI.Dialogs;
using Kassa.RxUI.Pages;
using Kassa.Shared;
using ReactiveUI;

namespace Kassa.Wpf.Pages;
/// <summary>
/// Interaction logic for EditDeliveryPage.xaml
/// </summary>
public partial class EditDeliveryPage : ReactiveUserControl<EditDeliveryPageVm>
{
    public EditDeliveryPage()
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

            this.OneWayBind(ViewModel, vm => vm.OrderEditPageVm.ShoppingList.Total, v => v.Price.Text, x => $"{x.ToString("0.##", RcsKassa.RuCulture)} ₽")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderEditPageVm.ShoppingList.ProductShoppingListItems, x => x.ShoppingListItems.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsDelivery, x => x.BasicAddressInfo.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsDelivery, x => x.AdditionalAddressInfo.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsDelivery, x => x.AddressNote.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsDelivery, x => x.Courier.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.CancelStatusButton.IsChecked, x => x == OrderStatus.Canceled)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.UnconfirmedStatusButton.IsChecked, x => x == OrderStatus.Unconfirmed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.CookingStatusButton.IsChecked, x => x == OrderStatus.InCooking)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.SendStatusButton.IsChecked, x => x == OrderStatus.OnTheWay)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.DeliveredStatusButton.IsChecked, x => x == OrderStatus.Completed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.OrderStatus, x => x.CompleatedStatusButton.IsChecked, x => x == OrderStatus.Completed)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.CancelStatusButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.UnconfirmedStatusButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.CookingStatusButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.SendStatusButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.DeliveredStatusButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, x => x.EditStatusCommand, x => x.CompleatedStatusButton)
                .DisposeWith(disposables);


            OrderEditPage.ViewModel = ViewModel.OrderEditPageVm;
            PaymentPage.ViewModel = ViewModel.PaymentPageVm;

            BackButton.Command = ViewModel.BackButtonCommand;
            Problem.Command = ViewModel.WriteProblemCommand;
            Save.Command = ViewModel.SaveCommand;
        });
    }
}
