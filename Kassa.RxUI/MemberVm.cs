﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public sealed class MemberVm : ReactiveObject, IApplicationModelPresenter<MemberDto>
{
    public Guid Id
    {
        get;
    }

    public MemberVm(MemberDto model)
    {
        Id = model.Id;
        Name = model.Name;

        SelectCommand = ReactiveCommand.Create(() =>
        {

            IsSelected = true;
        });
    }

    [Reactive]
    public string Name
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

    public void ModelChanged(Change<MemberDto> change)
    {
        var model = change.Current;

        Name = model.Name;
    }

    public void Dispose()
    {

    }

}