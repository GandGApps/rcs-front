﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.RxUI.Pages;
using ReactiveUI;

namespace Kassa.RxUI.Dialogs;
public sealed class TurnOffDialogViewModel : DialogViewModel
{
    private readonly IShiftService _shiftService;

    public TurnOffDialogViewModel(IShiftService shiftService)
    {
        _shiftService = shiftService;

        LogoutCommand = ReactiveCommand.CreateFromTask(async () =>
        {

            if (_shiftService.CurrentShift.Value == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Shift is not started");
            }

            await _shiftService.CurrentShift.Value!.Exit();

            await CloseAsync();
        });
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand
    {
        get;
    }
}
