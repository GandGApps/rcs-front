using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class ShiftRepository : IShiftRepository
{
    public Task Add(Shift item) => throw new NotImplementedException();
    public Task Delete(Shift item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public Task<Shift?> Get(Guid id) => throw new NotImplementedException();
    public Task<IEnumerable<Shift>> GetAll() => throw new NotImplementedException();
    public Task<IEnumerable<Shift>> GetShiftsForMember(Member member) => throw new NotImplementedException();
    public Task Update(Shift item) => throw new NotImplementedException();
}
