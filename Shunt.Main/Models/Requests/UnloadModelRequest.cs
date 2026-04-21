using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shunt.Main.Models.Requests;

public class UnloadModelRequest
{
    [JsonPropertyName("instance_id")]
    public string InstanceId { get; set; } = string.Empty;
}

[JsonSerializable(typeof(UnloadModelRequest))]
internal partial class UnloadModelRequestContext : JsonSerializerContext
{
    
}