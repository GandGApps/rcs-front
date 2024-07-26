using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.DataAccess.Model;
using Kassa.DataAccess.Repositories;
using Kassa.Shared;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class EmployeeRepository : IRepository<Member>
{
    public Task Add(Member item) => throw new NotImplementedException();
    public Task Delete(Member item) => throw new NotImplementedException();
    public Task DeleteAll() => throw new NotImplementedException();
    public async Task<Member?> Get(Guid id)
    {
        var employeeApi = Locator.Current.GetRequiredService<IEmployeeApi>();

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
        var employeeApi = Locator.Current.GetRequiredService<IEmployeeApi>();

        var employees = await employeeApi.GetMembers();

        if (employees == null)
        {
            return [];
        }

        return employees.Select(employee => new Member()
        {
            Id = employee.EmployeeId,
            Name = $"{employee.MainData.FirstName} {employee.MainData.MiddleName} {employee.MainData.LastName}"
        }).ToList();
    }
    public Task Update(Member item) => throw new NotImplementedException();
}
