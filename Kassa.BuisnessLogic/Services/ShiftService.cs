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
        CurrentShift = new ObservableOnlyBehaviourSubject<IShift?>(_currentShift);
        CurrentCashierShift = new ObservableOnlyBehaviourSubject<ITerminalShift?>((ITerminalShift?)null);
    }

    public IApplicationModelManager<ShiftDto> RuntimeShifts => _hostModelManager;

    public IObservableOnlyBehaviourSubject<IShift?> CurrentShift
    {
        get;
    }

    public IObservableOnlyBehaviourSubject<ITerminalShift?> CurrentCashierShift
    {
        get;
    }

    public async Task<bool> EnterPincode(string pincode)
    {
        if (_currentShift.Value is not null)
        {
            throw new InvalidOperationException("Shift is already started");
        }

        if (pincode == "0000")
        {
            var mockShift = new MockShift(this);

            if (MockShift._shift?.BreakStart != null)
            {
                await mockShift.EndBreak();
            }

            _currentShift.OnNext(mockShift);
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

    private sealed class MockShift : IShift
    {
        internal static ShiftDto? _shift;
        private readonly DateTime _start = DateTime.Now;
        private readonly BehaviorSubject<bool> _isStarted;
        private readonly ObservableOnlyBehaviourSubject<bool> _isStartedObservable;

        private static readonly MemberDto _mockUser = new()
        {
            Id = Guid.NewGuid(),
            Name = "Mock User"
        };
        private readonly ShiftService shiftService;

        public MockShift(ShiftService shiftService)
        {
            this.shiftService = shiftService;
            _isStarted = new(_shift is not null);
            _isStartedObservable = new(_isStarted);
        }

        public MemberDto Member => _mockUser;

        public IObservableOnlyBehaviourSubject<bool> IsStarted => _isStartedObservable;

        public Task Start()
        {
            _isStarted.OnNext(true);
            return Task.CompletedTask;
        }

        public async Task Exit()
        {
            var shiftDto = CreateDto();

            shiftDto.End = DateTime.Now;

            if (shiftDto.Id == Guid.Empty)
            {
                await shiftService.AddShift(shiftDto);
            }
            else
            {
                await shiftService.UpdateShift(shiftDto);
            }
            
            shiftService._currentShift.OnNext(null);
        }

        public async Task TakeBreak(string pincode)
        {
            var shiftDto = CreateDto();

            shiftDto.BreakStart = DateTime.Now;

            if (shiftDto.Id == Guid.Empty)
            {
                await shiftService.AddShift(shiftDto);
                shiftService._currentShift.OnNext(null);
                return;
            }

            await shiftService.UpdateShift(shiftDto);
        }

        public async Task EndBreak()
        {
            var shiftDto = CreateDto();

            shiftDto.BreakEnd = DateTime.Now;

            await shiftService.UpdateShift(shiftDto);
        }

        public async Task End(string pincode)
        {
            await Exit();
        }

        public ShiftDto CreateDto()
        {
            _shift ??= new ShiftDto()
            {
                ManagerId = null,
                MemberId = _mockUser.Id,
                Start = _start,
                Number = shiftService.RuntimeShifts.Count + 1
            };

            return _shift;
        }
    }
}
