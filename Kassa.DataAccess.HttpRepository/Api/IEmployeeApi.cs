using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IEmployeeApi : IUseMemberToken
{
    [Get("/employee")]
    public Task<IEnumerable<EmployeeResponse>> GetMembers();

    [Get("/employee/single")]
    public Task<EmployeeResponse?> GetMember([AliasAs("user_id")] Guid id);
}

internal sealed record EmployeeResponse([property: JsonPropertyName("employeeData")] EmployeeData EmployeeData);

internal sealed record EmployeeData([property: JsonPropertyName("employee_id")] Guid EmployeeId, [property: JsonPropertyName("mainData")] EmployeeMainData MainData, [property:JsonPropertyName("roles")] IEnumerable<string> Roles);

internal sealed record EmployeeMainData([property: JsonPropertyName("firstName")] string FirstName, [property: JsonPropertyName("middleName")] string MiddleName, [property: JsonPropertyName("LastName")] string LastName);