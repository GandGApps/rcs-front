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
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ShiftRepository : IShiftRepository, IEnableLogger
{
    private FrozenDictionary<Guid, Shift>? _shifts;

    public Task Add(Shift item) => throw new NotImplementedException();
    public Task Delete(Shift item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Shift?> Get(Guid id)
    {
        if (_shifts == null)
        {
            this.Log().Error("Shifts are not loaded");

            return Task.FromResult<Shift?>(null);
        }

        if (_shifts.TryGetValue(id, out var shift))
        {
            return Task.FromResult<Shift?>(shift);
        }

        return Task.FromResult<Shift?>(null);
    }
    public async Task<IEnumerable<Shift>> GetAll()
    {
        var api = Locator.Current.GetRequiredService<IEmployeePostApi>();

        var shiftsResponse = await api.GetPosts();

        var shifts = shiftsResponse.Select(ApiMapper.MapShiftResponseToShift).ToFrozenDictionary(x => x.Id);

        _shifts = shifts;

        return shifts.Values;

    }

    public async Task<IEnumerable<Shift>> GetShiftsForMember(Member member)
    {
        var api = Locator.Current.GetRequiredService<IEmployeePostApi>();

        var shiftsResponse = await api.GetPostsByEmployeeId(member.Id);

        var shifts = shiftsResponse.Select(ApiMapper.MapShiftResponseToShift).ToArray();

        return shifts;
    }

    public Task Update(Shift item) => throw new NotImplementedException();
}
