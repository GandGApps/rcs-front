﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.BuisnessLogic.Edgar.Api;
internal interface IEmployeePostsApi : IApiOfMemberToken
{
    [Post("/employee/posts")]
    public Task<IEnumerable<EmployeeResponsePost>> GetEmployeePosts(EmployeeGetPostsRequest request);

    [Post("/employee/open-post")]
    public Task OpenPost(EmployeeOpenPostRequest request);

    [Post("/employee/close-post")]
    public Task ClosePost(EmployeeClosePostRequest request);

    [Post("/employee/break-start")]
    public Task StartBreak(EmployeeBreakRequest request);

    [Post("/employee/break-end")]
    public Task EndBreak(EmployeeBreakRequest request);

    [Get("/employee/post-exists")]
    public Task<IEnumerable<EmployeeResponsePost>> GetEmployeePosts();
    [Post("/employee/create-post")]
    public Task CreatePost(EmployeeResponsePost request);

    [Post("/terminal/post-exists")]
    public Task PostExists(PostExistsResponse postExists);
}



internal sealed record EmployeeBreakRequest([property:JsonPropertyName("date")] DateTime Date, [property:JsonPropertyName("post_id")] Guid PostId);

internal sealed record EmployeeClosePostRequest([property:JsonPropertyName("closeDate")] DateTime CloseDate);

internal sealed record EmployeeOpenPostRequest([property:JsonPropertyName("openDate")] DateTime OpenDate, [property: JsonPropertyName("start_sum")] double StartSum);

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
    public DateTime OpenDate
    {
        get; set;
    }

    [JsonPropertyName("closeDate")]
    public DateTime CloseDate
    {
        get; set;
    }

    [JsonPropertyName("employee_id")]
    public Guid EmployeeId
    {
        get; set;
    }

    [JsonPropertyName("start_sum")]
    public double StartSum
    {
        get; set;
    }

    [JsonPropertyName("sales_sum")]
    public double SalesSum
    {
        get; set;
    }

    [JsonPropertyName("out_sum")]
    public double OutSum
    {
        get; set;
    }

    [JsonPropertyName("in_sum")]
    public double InSum
    {
        get; set;
    }

    [JsonPropertyName("seizure_sum")]
    public double SeizureSum
    {
        get; set;
    }

    [JsonPropertyName("pass_sum")]
    public double PassSum
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