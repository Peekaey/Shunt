using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shunt.Main.Models;

public class ModelListResponse
{
    [JsonPropertyName("models")]
    public List<ModelInfo> Models { get; set; } = new();
}

public class ModelInfo
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = string.Empty;
    
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
    
    [JsonPropertyName("architecture")]
    public string? Architecture { get; set; }
    
    [JsonPropertyName("quantization")]
    public QuantizationInfo Quantization { get; set; } = new();
    
    [JsonPropertyName("size_bytes")]
    public long SizeBytes { get; set; }
    
    [JsonPropertyName("params_string")]
    public string? ParamsString { get; set; }
    
    [JsonPropertyName("loaded_instances")]
    public List<LoadedInstance> LoadedInstances { get; set; } = new();
    
    [JsonPropertyName("max_context_length")]
    public int MaxContextLength { get; set; }
    
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;
    
    [JsonPropertyName("capabilities")]
    public CapabilitiesInfo? Capabilities { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("variants")]
    public List<string>? Variants { get; set; }
    
    [JsonPropertyName("selected_variant")]
    public string? SelectedVariant { get; set; }
}

public class QuantizationInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("bits_per_weight")]
    public int BitsPerWeight { get; set; }
}

public class LoadedInstance
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("config")]
    public InstanceConfig Config { get; set; } = new();
}

public class InstanceConfig
{
    [JsonPropertyName("context_length")]
    public int ContextLength { get; set; }
    
    [JsonPropertyName("eval_batch_size")]
    public int EvalBatchSize { get; set; }
    
    [JsonPropertyName("parallel")]
    public int Parallel { get; set; }
    
    [JsonPropertyName("flash_attention")]
    public bool FlashAttention { get; set; }
    
    [JsonPropertyName("offload_kv_cache_to_gpu")]
    public bool OffloadKvCacheToGpu { get; set; }
}

public class CapabilitiesInfo
{
    [JsonPropertyName("vision")]
    public bool Vision { get; set; }
    
    [JsonPropertyName("trained_for_tool_use")]
    public bool TrainedForToolUse { get; set; }
    
    [JsonPropertyName("reasoning")]
    public ReasoningInfo? Reasoning { get; set; }
}

public class ReasoningInfo
{
    [JsonPropertyName("allowed_options")]
    public List<string> AllowedOptions { get; set; } = new();
    
    [JsonPropertyName("default")]
    public string Default { get; set; } = string.Empty;
}

/// <summary>
/// Source-generated JSON context for ApiModels. 
/// Required for Native AOT to avoid reflection.
/// </summary>
[JsonSerializable(typeof(ModelListResponse))]
internal partial class ApiModelsContext : JsonSerializerContext
{
}