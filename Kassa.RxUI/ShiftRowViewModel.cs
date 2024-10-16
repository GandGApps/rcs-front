using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public sealed class ShiftRowViewModel : ReactiveObject, IApplicationModelPresenter<ShiftDto>
{
    private readonly IMemberService _memberService;

    public Guid Id
    {
        get;
    }

    public ShiftRowViewModel(ShiftDto shiftDto, IMemberService memberService)
    {
        _memberService = memberService;

        Id = shiftDto.Id;
        Number = shiftDto.Number;
        Name = _memberService.RuntimeMembers.TryGetValue(shiftDto.MemberId, out var member) ? member.Name : "???";
        Begin = shiftDto.Start?.ToString("dd.MM.yyyy | HH:mm") ?? string.Empty;
        End = shiftDto.End?.ToString("dd.MM.yyyy | HH:mm") ?? string.Empty;
        if (shiftDto.BreakStart != null)
        {
            if (shiftDto.BreakEnd.HasValue)
            {
                Break = $"{(shiftDto.BreakEnd.Value - shiftDto.BreakStart.Value).Minutes} минут";
            }
            else
            {
                // It's impossible to get here, because the break end is always set
                Break = $"Длиться все еще";

                this.Log().Error("Break end is not set for shift {ShiftId}", shiftDto.Id);
            }
        }
        else
        {
            Break = "Не было";
        }
        HourlyRate = shiftDto.HourlyRate;
        Earned = shiftDto.Earned;
        Fine = shiftDto.Fine;
        Manager = !shiftDto.ManagerId.HasValue ? "???" : _memberService.RuntimeMembers.TryGetValue(shiftDto.ManagerId.Value, out var manager) ? manager.Name : "???";
    }

    [Reactive]
    public int Number
    {
        get; set;
    }

    [Reactive]
    public string? Name
    {
        get; set;
    }

    [Reactive]
    public string Begin
    {
        get; set;
    }

    [Reactive]
    public string End
    {
        get; set;
    }

    [Reactive]
    public string Break
    {
        get; set;
    }

    [Reactive]
    public double HourlyRate
    {
        get; set;
    }

    [Reactive]
    public double Earned
    {
        get; set;
    }

    [Reactive]
    public double Fine
    {
        get; set;
    }

    [Reactive]
    public string? Comment
    {
        get; set;
    }

    [Reactive]
    public string Manager
    {
        get; set;
    }

    public void Dispose()
    {

    }

    public void ModelChanged(Change<ShiftDto> change)
    {
        var shiftDto = change.Current;

        Number = shiftDto.Number;
        Name = _memberService.RuntimeMembers.TryGetValue(shiftDto.MemberId, out var member) ? member.Name : "???";
        Begin = shiftDto.Start?.ToString("dd.MM.yyyy | HH:mm") ?? string.Empty;
        End = shiftDto.End?.ToString("dd.MM.yyyy | HH:mm") ?? string.Empty;
        if (shiftDto.BreakStart != null)
        {
            if (shiftDto.BreakEnd.HasValue)
            {
                Break = $"{(shiftDto.BreakEnd.Value - shiftDto.BreakStart.Value).Minutes} минут";
            }
            else
            {
                // It's impossible to get here, because the break end is always set
                Break = $"Длиться все еще";

                this.Log().Error("Break end is not set for shift {ShiftId}", shiftDto.Id);
            }
        }
        else
        {
            Break = "Не было";
        }
        HourlyRate = shiftDto.HourlyRate;
        Earned = shiftDto.Earned;
        Fine = shiftDto.Fine;
        Manager = !shiftDto.ManagerId.HasValue ? "???" : _memberService.RuntimeMembers.TryGetValue(shiftDto.ManagerId.Value, out var manager) ? manager.Name : "???";
    }
}
