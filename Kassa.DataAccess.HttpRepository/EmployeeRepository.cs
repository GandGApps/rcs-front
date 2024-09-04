using System;
using System.Collections.Generic;
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
internal sealed class EmployeeRepository : IRepository<Member>
{
    private readonly FrozenMemoryCache<Member> _frozenMemoryCache = new(TimeSpan.FromMinutes(15));

    public Task Add(Member item) => throw new NotImplementedException();
    public Task Delete(Member item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public async Task<Member?> Get(Guid id)
    {
        if (_frozenMemoryCache.IsExpired)
        {
            await GetAll();
        }

        if(_frozenMemoryCache.TryGetValue(id, out var cachedItem))
        {
            return cachedItem;
        }

        var employeeApi = RcsLocator.GetRequiredService<IEmployeeApi>();

        var employee = await employeeApi.GetMember(id);

        if (employee == null)
        {
            return null;
        }

        return new Member()
        {
            Id = employee.EmployeeData.EmployeeId,
            Name = $"{employee.EmployeeData.MainData.FirstName} {employee.EmployeeData.MainData.MiddleName} {employee.EmployeeData.MainData.LastName}",
            IsManager = employee.EmployeeData.Roles.Contains("manager", StringComparer.InvariantCultureIgnoreCase)
        };
    }
    public async Task<IEnumerable<Member>> GetAll()
    {
        if(!_frozenMemoryCache.IsExpired)
        {
            return _frozenMemoryCache.Values;
        }

        var employeeApi = RcsLocator.GetRequiredService<IEmployeeApi>();

        var employees = await employeeApi.GetMembers();

        if (employees == null)
        {
            return [];
        }

        var member = employees.Select(employee => new Member()
        {
            Id = employee.EmployeeId,
            Name = $"{employee.MainData.FirstName} {employee.MainData.MiddleName} {employee.MainData.LastName}",
            IsManager = employee.Roles.Contains("manager", StringComparer.InvariantCultureIgnoreCase)
        }).ToList();

        _frozenMemoryCache.Refresh(member);

        return member;
    }
    public Task Update(Member item) => throw new NotImplementedException();
}
