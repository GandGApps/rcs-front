using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI.Pages;
public sealed class OrderEditWithNavigationPageVm : PageViewModel
{
    private readonly ReadOnlyCollection<OrderEditDto> _orderEditDtos;
    private readonly Dictionary<OrderEditDto, OrderEditWithNavigationPageItemVm> _navigationItems = [];

    public ReactiveCommand<Unit, Unit> PreviousOrderEditCommand
    {
        get;
    }

    public ReactiveCommand<Unit, Unit> NextOrderEditCommand
    {
        get;
    }

    [Reactive]
    public int CurrentIndex
    {
        get; private set;
    }

    public extern OrderEditWithNavigationPageItemVm Current
    {
        [ObservableAsProperty]
        get;
    }

    public OrderEditWithNavigationPageVm(ReadOnlyCollection<OrderEditDto> orderEditDtos)
    {
        _orderEditDtos = orderEditDtos;


        this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => _navigationItems[_orderEditDtos[index]])
            .ToPropertyEx(this, x => x.Current)
            .DisposeWith(InternalDisposables);

        var previousOrderEditCommandValidation = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => index > 0);


        PreviousOrderEditCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
            }
        }, previousOrderEditCommandValidation).DisposeWith(InternalDisposables);

        var nextOrderEditCommandValidation = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(index => index < _orderEditDtos.Count - 1);

        NextOrderEditCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentIndex < _orderEditDtos.Count - 1)
            {
                CurrentIndex++;
            }
        }, nextOrderEditCommandValidation).DisposeWith(InternalDisposables);

        InternalDisposables.Add(Disposable.Create(() =>
        {
            foreach (var navigationItem in _navigationItems.Values)
            {
                navigationItem.Dispose();
            }
        }));
    }

}
