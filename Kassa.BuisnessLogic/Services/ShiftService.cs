using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.ApplicationModelManagers;
using Kassa.BuisnessLogic.Dto;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.BuisnessLogic.Services;
internal class ShiftService : BaseInitializableService, IShiftService
{
    private readonly IRepository<Shift> _repository;
    private readonly BehaviorSubject<IShift?> _currentShift = new(null);
    private readonly HostModelManager<ShiftDto> _hostModelManager = new();

    public ShiftService(IRepository<Shift> repository)
    {
        _hostModelManager.DisposeWith(InternalDisposables);
        _currentShift.DisposeWith(InternalDisposables);
        _repository = repository;
    }

    public IApplicationModelManager<ShiftDto> RuntimeShifts => _hostModelManager;

    public IObservable<IShift?> CurrentShift => _currentShift;

    public Task<bool> EnterPincode(string pincode)
    {
        if (_currentShift.Value is not null)
        {
            throw new InvalidOperationException("Shift is already started");
        }

        if (pincode == "0000")
        {
            _currentShift.OnNext(new MockShift(this));
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
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
            throw new InvalidOperationException($"Client with id {shift.Id} not found");
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

    private class MockShift(ShiftService shiftService) : IShift
    {
        private ShiftDto? _shift;
        private readonly DateTime _start = DateTime.Now;

        private static readonly UserDto _mockUser = new()
        {
            Id = Guid.NewGuid(),
            Name = "Mock User"
        };

        public UserDto User => _mockUser;

        public async Task Exit()
        {
            var shiftDto = await CreateDto();

            shiftDto.End = DateTime.Now;

            await shiftService.UpdateShift(shiftDto);
            shiftService._currentShift.OnNext(null);
        }

        public async Task TakeBreak()
        {
            var shiftDto = await CreateDto();

            shiftDto.BreakStart = DateTime.Now;

            await shiftService.UpdateShift(shiftDto);
        }

        public async Task EndBreak()
        {
            var shiftDto = await CreateDto();

            shiftDto.BreakEnd = DateTime.Now;

            await shiftService.UpdateShift(shiftDto);
        }

        public ValueTask<ShiftDto> CreateDto()
        {
            _shift ??= new ShiftDto()
            {
                Id = Guid.NewGuid(),
                ManagerId = null,
                UserId = _mockUser.Id,
                Start = _start,
                Number = shiftService.RuntimeShifts.Count + 1
            };

            return new(_shift);
        }
    }
}
