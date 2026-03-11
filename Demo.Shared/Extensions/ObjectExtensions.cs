using Demo.Shared.Utilities;
using System.Text.Json;

namespace Demo.Shared.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Serializes an object to JSON
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string ToJson(this object obj)
    {
        string json = JsonSerializer.Serialize(obj, Utils.JsonSerializerOptionsForClone);

        return json;
    }

    /// <summary>
    /// Clones an object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="source">Object to clone</param>
    /// <returns>New object</returns>
    public static T? Clone<T>(this T source)
    {
        string serialized = source?.ToJson() ?? string.Empty;
        T? deserialzed = JsonSerializer.Deserialize<T>(serialized);

        return deserialzed;
    }
}
