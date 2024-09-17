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
internal interface IEmployeePostApi: IUseMemberToken
{
    [Get("/employee/all-posts")]
    Task<IEnumerable<ShiftResponse>> GetPosts();

    [Get("/employee/posts-byid")]
    Task<IEnumerable<ShiftResponse>> GetPostsByEmployeeId([AliasAs("employee_id")] Guid id);
}


internal sealed record ShiftResponse(
    [property: JsonPropertyName("employeepost_id")] Guid EmployeePostId,
    [property: JsonPropertyName("terminal_id")] Guid TerminalId,
    [property: JsonPropertyName("schedule")] string? Schedule,
    [property: JsonPropertyName("openDate")] DateTime? OpenDate,
    [property: JsonPropertyName("closeDate")] DateTime? CloseDate,
    [property: JsonPropertyName("employee_id")] Guid EmployeeId,
    [property: JsonPropertyName("terminal_shift_id")] Guid? TerminalShiftId,
    [property: JsonPropertyName("manager")] Guid? Manager,
    [property: JsonPropertyName("start_sum")] decimal? StartSum,
    [property: JsonPropertyName("sales_sum")] decimal? SalesSum,
    [property: JsonPropertyName("out_sum")] decimal? OutSum,
    [property: JsonPropertyName("in_sum")] decimal? InSum,
    [property: JsonPropertyName("seizure_sum")] decimal? SeizureSum,
    [property: JsonPropertyName("pass_sum")] decimal? PassSum,
    [property: JsonPropertyName("office_id")] Guid OfficeId,
    [property: JsonPropertyName("isOpen")] bool IsOpen,
    [property: JsonPropertyName("onBreak")] bool? OnBreak,
    [property: JsonPropertyName("breakStart")] DateTime? BreakStart,
    [property: JsonPropertyName("breakEnd")] DateTime? BreakEnd,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTime UpdatedAt,
    [property: JsonPropertyName("post_number")] int? Number
);