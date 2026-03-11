using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo.Shared.Utilities;

public static class Utils
{
    /// <summary>
    /// Default JSON serializer options
    /// </summary>
    public static JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static JsonSerializerOptions JsonSerializerOptionsForClone = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };
}
