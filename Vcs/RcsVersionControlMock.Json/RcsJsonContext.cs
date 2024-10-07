using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RcsVersionControlMock.DataAccess;

namespace RcsVersionControlMock.Json;

[JsonSerializable(typeof(IEnumerable<Version>))]
[JsonSerializable(typeof(IEnumerable<VersionChangeNode>))]
[JsonSerializable(typeof(IEnumerable<VersionChanges>))]
public sealed partial class RcsJsonContext: JsonSerializerContext
{
}
