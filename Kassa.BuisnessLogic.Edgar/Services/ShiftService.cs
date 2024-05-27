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

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ShiftService : BaseInitializableService, IShiftService
{
    private readonly IRepository<Shift> _repository;
    private readonly IMemberService _memberService;
    internal readonly BehaviorSubject<IShift?> _currentShift = new(null);
    internal readonly BehaviorSubject<ITerminalShift?> _currentCashierShift = new(null);
    private readonly HostModelManager<ShiftDto> _hostModelManager = new();
    private readonly IAuthService _authService;

    public ShiftService(IRepository<Shift> repository, IMemberService memberService, IAuthService authService)
    {
        _hostModelManager.DisposeWith(InternalDisposables);
        _currentShift.DisposeWith(InternalDisposables);
        _repository = repository;
        _memberService = memberService;
        CurrentShift = new(_currentShift);
        CurrentCashierShift = new(_currentCashierShift);
        _authService = authService;
    }

    public IApplicationModelManager<ShiftDto> RuntimeShifts => _hostModelManager;

    public ObservableOnlyBehaviourSubject<IShift?> CurrentShift
    {
        get;
    }

    public ObservableOnlyBehaviourSubject<ITerminalShift?> CurrentCashierShift
    {
        get;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await GetShifts();

        _authService.CurrentAuthenticationContext.Subscribe(async context =>
        {

            if (context.Member is MemberDto member)
            {
                await FetchShiftDetails(member);

                await FetchCashierShiftDetails(member);
            }

        }).DisposeWith(disposables);
    }

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
        var shift = await _repository.Get(id);

        if (shift is null)
        {
            throw new InvalidOperationException($"Client with id {id} not found");
        }

        await _repository.Delete(shift);

        RuntimeShifts.Remove(id);
    }

    public async Task UpdateShift(ShiftDto shift)
    {
        var foundedShift = await _repository.Get(shift.Id);

        if (foundedShift is null)
        {
            throw new InvalidOperationException($"Shift with id {shift.Id} not found");
        }

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

    private async Task FetchShiftDetails(MemberDto member)
    {
        var employeePostsApi = Locator.GetRequiredService<IEmployeePostsApi>();

        var exist = await employeePostsApi.PostExists(new(DateTime.Now));

        if (!this.IsCashierShiftStarted() && !member.IsManager)
        {
            throw new InvalidUserOperatationException("Кассовая смена не открыта") { Description = "Дождитесь менеджера" };
        }

        var shift = new EdgarShift(this, member, exist.CreatedPost.IsOpen, exist);

        if (exist.Exists && exist.CreatedPost.IsBreakNotEnded)
        {
            await shift.EndBreak();
        }

        _currentShift.OnNext(shift);
    }

    private async Task FetchCashierShiftDetails(MemberDto member)
    {
        var config = Locator.GetRequiredService<IConfiguration>();

        var terminalApi = Locator.GetRequiredService<ITerminalPostApi>();

        var postExists = await terminalApi.PostExists(new(DateTime.Now));

        var managerShift = new EdgarTerminalShift(member, postExists, this);

        if (!this.IsCashierShiftStarted() && !member.IsManager)
        {

            throw new InvalidUserOperatationException("Кассовая смена не открыта") { Description = "Дождитесь менеджера" };
        }

        _currentCashierShift.OnNext(managerShift);
    }

}
