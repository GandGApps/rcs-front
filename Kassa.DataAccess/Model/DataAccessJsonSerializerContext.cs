using System.Text.Json.Serialization;
using Kassa.Shared;

namespace Kassa.DataAccess.Model;

[JsonSerializable(typeof(IEnumerable<Additive>))]
[JsonSerializable(typeof(IEnumerable<CashierShift>))]
[JsonSerializable(typeof(IEnumerable<Category>))]
[JsonSerializable(typeof(IEnumerable<Client>))]
[JsonSerializable(typeof(IEnumerable<ContributionReason>))]
[JsonSerializable(typeof(IEnumerable<Courier>))]
[JsonSerializable(typeof(IEnumerable<District>))]
[JsonSerializable(typeof(IEnumerable<FastMenu>))]
[JsonSerializable(typeof(IEnumerable<Ingredient>))]
[JsonSerializable(typeof(IEnumerable<Member>))]
[JsonSerializable(typeof(IEnumerable<Order>))]
[JsonSerializable(typeof(PaymentType))]
[JsonSerializable(typeof(IEnumerable<Product>))]
[JsonSerializable(typeof(IEnumerable<Receipt>))]
[JsonSerializable(typeof(IEnumerable<SeizureReason>))]
[JsonSerializable(typeof(IEnumerable<Shift>))]
[JsonSerializable(typeof(IEnumerable<Street>))]
[JsonSerializable(typeof(IEnumerable<User>))]
public sealed partial class DataAccessJsonSerializerContext : JsonSerializerContext
{
}