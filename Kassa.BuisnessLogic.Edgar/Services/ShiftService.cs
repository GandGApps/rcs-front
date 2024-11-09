using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.BuisnessLogic.Edgar.Api;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ShiftService : BaseInitializableService, IShiftService
{
    private readonly IRepository<Shift> _repository;
    private readonly IMemberService _memberService;
    private readonly IAuthService _authService;
    private readonly IEmployeePostsApi _employeePostsApi;
    private readonly ITerminalPostApi _terminalPostApi;

    internal readonly AdapterBehaviorSubject<IShift?> _currentShift = new(null);
    internal readonly AdapterBehaviorSubject<ITerminalShift?> _currentCashierShift = new(null);
    private readonly HostModelManager<ShiftDto> _hostModelManager = new();
    

    public ShiftService(IRepository<Shift> repository, IMemberService memberService, IAuthService authService, IEmployeePostsApi employeePostsApi, ITerminalPostApi terminalPostApi)
    {
        _repository = repository;
        _memberService = memberService;
        _authService = authService;
        _employeePostsApi = employeePostsApi;
        _terminalPostApi = terminalPostApi;

        _hostModelManager.DisposeWith(InternalDisposables);
        _currentShift.DisposeWith(InternalDisposables);
        

        _authService.CurrentAuthenticationContext.Subscribe(async context =>
        {

            if (context.Member is MemberDto member)
            {
                var managerShift = await FetchCashierShiftDetails(member);

                await FetchShiftDetails(member, managerShift);

                await GetShifts();
            }

        }).DisposeWith(InternalDisposables);
    }

    public IApplicationModelManager<ShiftDto> RuntimeShifts => _hostModelManager;

    public IObservableOnlyBehaviourSubject<IShift?> CurrentShift => _currentShift;
    public IObservableOnlyBehaviourSubject<ITerminalShift?> CurrentCashierShift => _currentCashierShift;

    public async ValueTask<ShiftDto?> GetShiftById(Guid id)
    {
        var shift = await _repository.Get(id);

        if (shift is null)
        {
            return null;
        }

        var shiftDto = Mapper.MapShiftToDto(shift);

        RuntimeShifts.AddOrUpdate(shiftDto);

        return shiftDto;
    }

    public async Task AddShift(ShiftDto shift)
    {
        shift.Id = Guid.NewGuid();

        var newShift = Mapper.MapDtoToShift(shift);

        await _repository.Add(newShift);

        RuntimeShifts.AddOrUpdate(shift);
    }

    public async Task DeleteShift(Guid id)
    {
        var shift = await _repository.Get(id) ?? ThrowHelper.ThrowInvalidOperationException<Shift>($"Client with id {id} not found");
        await _repository.Delete(shift);

        RuntimeShifts.Remove(id);
    }

    public async Task UpdateShift(ShiftDto shift)
    {
        var foundedShift = await _repository.Get(shift.Id) ?? ThrowHelper.ThrowInvalidOperationException<Shift>($"Shift with id {shift.Id} not found");
        var updatedShift = Mapper.MapDtoToShift(shift);

        await _repository.Update(updatedShift);

        RuntimeShifts.AddOrUpdate(shift);
    }

    public async Task<IEnumerable<ShiftDto>> GetShifts()
    {
        var shifts = (await _repository.GetAll()).Select(Mapper.MapShiftToDto);

        RuntimeShifts.AddOrUpdate(shifts);

        return shifts;
    }

    private async Task FetchShiftDetails(MemberDto member, EdgarTerminalShift edgarTerminalShift)
    {
        var managerShift = edgarTerminalShift.CreateDto();

        var exist = await _employeePostsApi.PostExists(new(DateTime.Now, managerShift.Id));

        if (!this.IsCashierShiftStarted() && !member.IsManager)
        {
            InvalidUserOperatationException.Throw("Кассовая смена не открыта", "Дождитесь менеджера");
        }

        var shift = new EdgarShift(this, member, exist.CreatedPost.IsOpen, exist);

        if (exist.Exists && exist.CreatedPost.IsBreakNotEnded)
        {
            await shift.EndBreak();
        }

        _currentShift.OnNext(shift);
    }

    private async Task<EdgarTerminalShift> FetchCashierShiftDetails(MemberDto member)
    {
        var postExists = await _terminalPostApi.PostExists(new(DateTime.Now));

        var managerShift = RcsKassa.CreateAndInject<EdgarTerminalShift>(member, postExists, this);

        if (!this.IsCashierShiftStarted() && !member.IsManager)
        {
            InvalidUserOperatationException.Throw("Кассовая смена не открыта", "Дождитесь менеджера");
        }

        _currentCashierShift.OnNext(managerShift);

        return managerShift;
    }

}
