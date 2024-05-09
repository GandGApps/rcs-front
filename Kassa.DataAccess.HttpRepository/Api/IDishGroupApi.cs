﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Refit;

namespace Kassa.DataAccess.HttpRepository.Api;
internal interface IDishGroupApi : IApiOfMemberToken
{
    [Get("/dishes/groups")]
    public Task<IEnumerable<DishGroupRequest>> GetDishGroups();
}


internal sealed record DishGroupRequest
{
    [property: JsonPropertyName("group_model_id")]
    public Guid GroupModelId
    {
        get; set;
    }

    [property: JsonPropertyName("nomenclatureCode")]
    public int NomenclatureCode
    {
        get; set;
    }

    [property: JsonPropertyName("isParentGroup")]
    public bool IsParentGroup
    {
        get; set;
    }

    [property: JsonPropertyName("parentGroupId")]
    public Guid? ParentGroupId
    {
        get; set;
    }

    [property: JsonPropertyName("title")]
    public string Title
    {
        get; set;
    }

    [property: JsonPropertyName("office_id")]
    public Guid OfficeId
    {
        get; set;
    }
}

// The dish group request model.

/*class GroupModel extends Model { }

GroupModel.init(
    {
        group_model_id: {
            type: DataTypes.UUID,
            allowNull: true,
            primaryKey: true,
            defaultValue: DataTypes.UUIDV4,
            validate: {
                notEmpty: true,
            },
        },
        nomenclatureCode: {
            type: DataTypes.INTEGER
        },
        isParentGroup: {
            type: DataTypes.BOOLEAN
        },
        parentGroupId: {
            type: DataTypes.UUID,
            allowNull: true
        },
        title: {
            type: DataTypes.STRING
        },
        office_id: {
            type: DataTypes.UUID,
            allowNull: true,
            references: {
                model: 'office',
                key: 'office_id'
            }
        }
    },
    {
        modelName: "groupmodel",
        tableName: "groupmodel",
        sequelize,
    }
);*/