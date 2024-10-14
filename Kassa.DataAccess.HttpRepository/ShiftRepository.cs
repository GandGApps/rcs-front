using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ShiftRepository : IShiftRepository, IEnableLogger
{
    private readonly FrozenMemoryCache<Shift> _cache = new();
    private readonly IEmployeePostApi _api;

    public ShiftRepository(IEmployeePostApi api)
    {
        _api = api;
    }

    public Task Add(Shift item) => throw new NotImplementedException();
    public Task Delete(Shift item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public async Task<Shift?> Get(Guid id)
    {
        if (_cache.IsExpired)
        {
            await GetAll();
        }

        if (_cache.TryGetValue(id, out var shift))
        {
            return shift;
        }

        return null;
    }
    public async Task<IEnumerable<Shift>> GetAll()
    {
        if (!_cache.IsExpired)
        {
            return _cache.Values;
        }

        var shiftsResponse = await _api.GetPosts();

        var shifts = shiftsResponse.Select(ApiMapper.MapShiftResponseToShift).ToList();

        _cache.Refresh(shifts);

        return shifts;

    }

    public async Task<IEnumerable<Shift>> GetShiftsForMember(Member member)
    {
        var shiftsResponse = await _api.GetPostsByEmployeeId(member.Id);

        var shifts = shiftsResponse.Select(ApiMapper.MapShiftResponseToShift).ToArray();

        return shifts;
    }

    public Task Update(Shift item) => throw new NotImplementedException();
}
