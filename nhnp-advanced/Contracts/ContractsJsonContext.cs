using System.Text.Json.Serialization;

namespace Contracts;

[JsonSerializable(typeof(PluginRequest))]
public partial class ContractsJsonContext : JsonSerializerContext
{
}
