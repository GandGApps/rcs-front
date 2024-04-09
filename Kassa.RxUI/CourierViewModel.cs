using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.RxUI.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class CourierViewModel : ReactiveObject, IApplicationModelPresenter<CourierDto>
{
    private readonly CompositeDisposable disposables = [];
    private CourierDto _source;

    public CourierViewModel(CourierDto courier)
    {
        _source = courier;

        Id = courier.Id;
        FirstName = courier.FirstName;
        LastName = courier.LastName;
        MiddleName = courier.MiddleName;
        Phone = courier.Phone;

        SelectCommand = ReactiveCommand.Create(() =>
        {
        }).DisposeWith(disposables);

        this.WhenAnyValue(x => x.FirstName, x => x.MiddleName, x => x.LastName, (firstName, middleName, lastName) => $"{FirstName} {MiddleName} {LastName}")
            .ToPropertyEx(this, x => x.FullName)
            .DisposeWith(disposables);
    }

    public CourierViewModel(CourierDto courier, SearchCourierDialogViewModel couriers): this(courier)
    {

        couriers
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SearchText)
            .Select(x => !string.IsNullOrWhiteSpace(x))
            .ToPropertyEx(this, x => x.IsDetailsVisible)
            .DisposeWith(disposables);

        couriers
            .WhenAnyValue(allClientsDialogView => allClientsDialogView.SelectedItem)
            .Subscribe(x =>
            {
                IsSelected = x != null && x.Id == Id;
            });

        SelectCommand = ReactiveCommand.Create(() =>
        {
            couriers.SelectedItem = this;
        });
    }


    public Guid Id
    {
        get;
    }
    [Reactive]
    public string FirstName
    {
        get; set;
    }
    [Reactive]
    public string LastName
    {
        get; set;
    }
    [Reactive]
    public string MiddleName
    {
        get; set;
    }
    [Reactive]
    public string Phone
    {
        get; set;
    }
    [Reactive]
    public bool IsSelected
    {
        get; set;
    }

    public ReactiveCommand<Unit, Unit> SelectCommand
    {
        get;
    }

    
    public extern string FullName
    {
        [ObservableAsProperty]
        get;
    }

    public extern bool IsDetailsVisible
    {
        [ObservableAsProperty]
        get;
    }

    public void Dispose() => disposables.Dispose();

    public void ModelChanged(Change<CourierDto> change)
    {
        _source = change.Current;

        FirstName = _source.FirstName;
        LastName = _source.LastName;
        MiddleName = _source.MiddleName;
        Phone = _source.Phone;
    }

    public override string ToString() => FullName;
}
