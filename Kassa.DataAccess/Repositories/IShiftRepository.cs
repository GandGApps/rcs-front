using System;
using System.Collections.Generic;
using System.Text;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess.Repositories;
public interface IShiftRepository : IRepository<Shift>
{

    public Task<IEnumerable<Shift>> GetShiftsForMember(Member member);

    internal sealed class MockShiftRepository(IRepository<Shift> repository) : BasicMockRepository(repository), IShiftRepository
    {

        public Task<IEnumerable<Shift>> GetShiftsForMember(Member member)
        {
            var shifts = GetAll().Result.Where(shift => shift.MemberId == member.Id);

            return Task.FromResult(shifts);
        }
    }

}
