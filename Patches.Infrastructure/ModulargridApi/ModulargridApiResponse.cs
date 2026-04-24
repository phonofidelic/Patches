using System.Text.Json.Serialization;

namespace Patches.Infrastructure.ModulargridApi;

internal class ModulargridApiResponse
{
    [JsonPropertyName("response")]
    public ModulargridResponseWrapper? Response { get; set; }
}

internal class ModulargridResponseWrapper
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("result")]
    public ModulargridResult? Result { get; set; }
}

internal class ModulargridResult
{
    [JsonPropertyName("Module")]
    public List<ModulargridModule>? Module { get; set; }
}

internal class ModulargridModule
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("te")]
    public string? Te { get; set; }

    [JsonPropertyName("Vendor")]
    public ModulargridVendor? Vendor { get; set; }
}

internal class ModulargridVendor
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
