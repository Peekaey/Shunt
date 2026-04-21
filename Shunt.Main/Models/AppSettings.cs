using System.Net;
using System.Text.Json.Serialization;

namespace Shunt.Main.Models;

public class AppSettings
{
    public int ServerPort { get; set; }
    public string ServerIp { get; set; }
    public string DefaultModel { get; set; }
    public int ContextTokenLength { get; set; }
}

[JsonSerializable((typeof(AppSettings)))]
internal partial class AppSettingsContext : JsonSerializerContext
{
    
}