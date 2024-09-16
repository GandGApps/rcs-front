using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.Shared;

namespace Kassa.DataAccess.HttpRepository;

[JsonSerializable(typeof(IEnumerable<AdditiveEdgarModel>))]
[JsonSerializable(typeof(IEnumerable<DishRequest>))]
[JsonSerializable(typeof(IEnumerable<DishGroupRequest>))]
[JsonSerializable(typeof(IEnumerable<EmployeeResponse>))]
[JsonSerializable(typeof(IEnumerable<EmployeeData>))]
[JsonSerializable(typeof(IEnumerable<ShiftResponse>))]
[JsonSerializable(typeof(ContributionResponse[]))]
[JsonSerializable(typeof(SeizureResponse[]))]
[JsonSerializable(typeof(IEnumerable<IngredientResponse>))]
[JsonSerializable(typeof(IEnumerable<EdgarOrderInfoResponse>))]
[JsonSerializable(typeof(IEnumerable<OrderEdgarModel>))]
[JsonSerializable(typeof(IEnumerable<TechcardEdgarResponse>))]
internal sealed partial class DataAccessHttpRepositoryJsonSerializerContext: JsonSerializerContext
{

}
