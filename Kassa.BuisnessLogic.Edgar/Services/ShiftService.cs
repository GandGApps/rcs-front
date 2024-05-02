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
using Kassa.BuisnessLogic.Edgar.Dto;
using Kassa.BuisnessLogic.Services;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;

namespace Kassa.BuisnessLogic.Edgar.Services;
internal sealed class ShiftService : BaseInitializableService, IShiftService
{
    private readonly IRepository<Shift> _repository;
    internal readonly BehaviorSubject<IShift?> _currentShift = new(null);
    private readonly HostModelManager<ShiftDto> _hostModelManager = new();

    public ShiftService(IRepository<Shift> repository, IRepository<Member> members)
    {
        _hostModelManager.DisposeWith(InternalDisposables);
        _currentShift.DisposeWith(InternalDisposables);
        _repository = repository;
        CurrentShift = new(_currentShift);
    }

    public IApplicationModelManager<ShiftDto> RuntimeShifts => _hostModelManager;

    public ObservableOnlyBehaviourSubject<IShift?> CurrentShift
    {
        get;
    }

    public async Task<bool> EnterPincode(string pincode)
    {
        var pincodeRequest = new EnterPincodeRequest(pincode);

        var terminalApi = Locator.GetRequiredService<ITerminalApi>();

        var reponse = await terminalApi.EnterPincode(pincodeRequest);

        if (reponse.IsSuccessStatusCode)
        {
            var pincodeResponse = reponse.Content!;

            var config = Locator.GetRequiredService<IConfiguration>();
            config["MemberAuthToken"] = pincodeResponse.Token;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(pincodeResponse.Token);

            var employeeId = token.Claims.First(claim => claim.Type == "employee_id").Value;


            _currentShift.OnNext(new EdgarShift(this,new()));

            return true;
        }

        return false;
    }

    protected async override ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        await GetShifts();
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

}
