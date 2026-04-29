using System.Text.Json.Serialization;

namespace Shunt.Main.Models.Requests;

public class LoadModelRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("context_length")]
    public int ContextLength { get; set; }
    [JsonPropertyName("eval_batch_size")]
    public string EvalBatchSize { get; set; }
    [JsonPropertyName("flash_attention")]
    public string FlashAttention { get; set; }
    [JsonPropertyName("num_experts")]
    public string NumExpert { get; set; }
    [JsonPropertyName("offload_kv_cache_to_gpu")]
    public string OffloadKvCacheToGpu { get; set; }
    [JsonPropertyName("echo_load_config")]
    public bool EchoLoadConfig { get; set; }
}

[JsonSerializable(typeof(LoadModelRequest))]
internal partial class LoadModelRequestContext : JsonSerializerContext
{
    
}