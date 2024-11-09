using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class EmployeeRepository : IRepository<Member>
{
    private readonly FrozenMemoryCache<Member> _frozenMemoryCache = new();
    private readonly IEmployeeApi _api;

    public EmployeeRepository(IEmployeeApi api)
    {
        _api = api;
    }

    public Task Add(Member item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task Delete(Member item) => ThrowHelper.ThrowNotSupportedException<Task>();
    public Task DeleteAll() => ThrowHelper.ThrowNotSupportedException<Task>();
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

        var employee = await _api.GetMember(id);

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

        var employees = await _api.GetMembers();

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
    public Task Update(Member item) => ThrowHelper.ThrowNotSupportedException<Task>();
}
