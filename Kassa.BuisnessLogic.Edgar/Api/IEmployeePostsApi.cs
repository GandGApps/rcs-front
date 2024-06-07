using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.Shared.DelegatingHandlers;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;
internal interface IEmployeePostsApi : IUseMemberToken
{
    [Post("/employee/posts")]
    public Task<IEnumerable<EmployeeResponsePost>> GetEmployeePosts(EmployeeGetPostsRequest request);

    [Post("/employee/open-post")]
    public Task<OpenPostRequest> OpenPost(EmployeeOpenPostRequest request);

    [Post("/employee/close-post")]
    public Task ClosePost(EmployeeClosePostRequest request);

    [Post("/employee/break-start")]
    public Task StartBreak(EmployeeBreakRequest request);

    [Post("/employee/break-end")]
    public Task EndBreak(EmployeeBreakRequest request);

    [Post("/employee/create-post")]
    public Task CreatePost(EmployeeResponsePost request);

    [Post("/employee/post-exists")]
    public Task<PostExistsResponse> PostExists(PostExistsRequest postExists);
}

internal sealed record OpenPostRequest([property:JsonPropertyName("message")] string Message);

internal sealed record PostExistsResponse
{
    [JsonPropertyName("exists")]
    public bool Exists
    {
        get; init;
    }

    [JsonPropertyName("posts")]
    public CreatedPost CreatedPost
    {
        get; init;
    }
}

internal sealed record CreatedPost
{
    [JsonPropertyName("employeepost_id")]
    public Guid PostId
    {
        get; init;
    }

    [JsonPropertyName("terminal_id")]
    public Guid TerminalId
    {
        get; init;
    }

    [JsonPropertyName("openDate")]
    public DateTime? OpenDate
    {
        get; init;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; init;
    }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt
    {
        get; init;
    }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt
    {
        get; init;
    }

    [JsonPropertyName("isOpen")]
    public bool IsOpen
    {
        get; init;
    }

    [JsonPropertyName("breakStart")]
    public DateTime? BreakStart
    {
        get; init;
    }

    [JsonPropertyName("breakEnd")]
    public DateTime? BreakEnd
    {
        get; init;
    }

    public bool IsBreakNotEnded => BreakStart.HasValue && !BreakEnd.HasValue;

    [JsonPropertyName("manager")]
    public Guid? ManagerId
    {
        get; init;
    }

    [JsonPropertyName("terminal_shift_id")]
    public Guid TerminalShiftId
    {
        get; init;
    }
}

internal sealed record PostExistsRequest([property:JsonPropertyName("openDate")] DateTime CurrentDate, [property:JsonPropertyName("terminal_shift_id")] Guid TerminalShiftId);

internal sealed record EmployeeBreakRequest([property:JsonPropertyName("date")] DateTime Date, [property:JsonPropertyName("post_id")] Guid PostId);

internal sealed record EmployeeClosePostRequest([property:JsonPropertyName("closeDate")] DateTime CloseDate, [property: JsonPropertyName("post_id")] Guid PostId);

internal sealed record EmployeeOpenPostRequest([property:JsonPropertyName("openDate")] DateTime OpenDate,[property: JsonPropertyName("post_id")] Guid PostId, [property: JsonPropertyName("start_sum")] double StartSum, [property: JsonPropertyName("terminal_shift_id")] Guid TerminalShiftId);

internal sealed record EmployeeGetPostsRequest([property:JsonPropertyName("date")] DateTime Date);

internal sealed record EmployeeResponsePost
{
    [JsonPropertyName("terminal_id")]
    public Guid TerminalId
    {
        get; set;
    }

    [JsonPropertyName("schedule")]
    public string Schedule
    {
        get; set;
    }

    [JsonPropertyName("openDate")]
    public DateTime? OpenDate
    {
        get; set;
    }

    [JsonPropertyName("closeDate")]
    public DateTime? CloseDate
    {
        get; set;
    }

    [JsonPropertyName("employee_id")]
    public Guid EmployeeId
    {
        get; set;
    }

    [JsonPropertyName("start_sum")]
    public double? StartSum
    {
        get; set;
    }

    [JsonPropertyName("sales_sum")]
    public double? SalesSum
    {
        get; set;
    }

    [JsonPropertyName("out_sum")]
    public double? OutSum
    {
        get; set;
    }

    [JsonPropertyName("in_sum")]
    public double? InSum
    {
        get; set;
    }

    [JsonPropertyName("seizure_sum")]
    public double? SeizureSum
    {
        get; set;
    }

    [JsonPropertyName("pass_sum")]
    public double? PassSum
    {
        get; set;
    }

    [JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; set;
    }

    [JsonPropertyName("isOpen")]
    public bool IsOpen
    {
        get; set;
    }
}

// This is the employee post request model.

/*class EmployeePostModel extends Model { }

EmployeePostModel.set(
    {
        employeepost_id: {
            type: DataTypes.UUID,
            allowNull: true,
            primaryKey: true,
            defaultValue: DataTypes.UUIDV4,
            validate: {
                notEmpty: true,
            },
        },
        terminal_id: {
            type: DataTypes.UUID,
            allowNull: true,
            references: {
                model: 'terminal',
                key: 'terminal_id'
            }
        },
        schedule: {
            type: DataTypes.STRING
        },
        openDate: {
            type: DataTypes.DATE
        },
        closeDate: {
            type: DataTypes.DATE
        },
        employee_id: {
            type: DataTypes.UUID,
            allowNull: true,
            references: {
                model: 'employee',
                key: 'employee_id'
            }
        },
        start_sum: {
            type: DataTypes.DOUBLE
        },
        sales_sum: {
            type: DataTypes.DOUBLE
        },
        out_sum: {
            type: DataTypes.DOUBLE
        },
        in_sum: {
            type: DataTypes.DOUBLE
        },
        seizure_sum: {
            type: DataTypes.DOUBLE
        },
        pass_sum: {
            type: DataTypes.DOUBLE
        },
        office_id: {
            type: DataTypes.UUID,
            allowNull: true,
            references: {
                model: 'office',
                key: 'office_id'
            }
        },
        isOpen: {
            type: DataTypes.BOOLEAN
        }
    },
    {
        modelName: "employeepost",
        tableName: "employeepost",
        sequelize,
    }
);*/