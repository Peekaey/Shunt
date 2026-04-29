using System.Text.Json.Serialization;

namespace Shunt.Main.Models;

public class CachedAppSettings
{
    public int ServerPort { get; set; } = 1234;
    public string ServerIp { get; set; }  = "http://127.0.0.1";
    public string DefaultModel { get; set; } = "gemma-4-e2b";
    public int ContextTokenLength { get; set; } = 4096;
    public bool EnableAutoLoadUnload { get; set; } = true;
    public string ApiKey { get; set; } = string.Empty;
}

[JsonSerializable((typeof(AppSettings)))]
internal partial class CachedAppSettingsContext : JsonSerializerContext
{
    
}